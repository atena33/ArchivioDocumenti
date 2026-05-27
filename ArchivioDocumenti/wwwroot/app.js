const API_URL = '/api/documenti';
let modalAllegatoIstanza = null;
let modalNuovoDocIstanza = null;

const STATO_ENUM_MAP = {
    'bozza': 0,
    'pubblicato': 1,
    'archiviato': 2,
    'annullato': 3
};

document.addEventListener('DOMContentLoaded', () => {
    modalAllegatoIstanza = new bootstrap.Modal(document.getElementById('modalAllegato'));
    modalNuovoDocIstanza = new bootstrap.Modal(document.getElementById('modalNuovoDocumento'));

    caricaTuttiIDocumenti();
    caricaCategorieSorgenti(); 
    caricaClientiSorgenti();   
});

async function caricaCategorieSorgenti() {
    try {
        const response = await fetch('/api/categorie');
        const categorie = await response.json();
        const selectCat = document.getElementById('nuovaCategoria');

        categorie.forEach(cat => {
            selectCat.innerHTML += `<option value="${cat.id}">${cat.nome}</option>`;
        });
    } catch (error) {
        console.error("Errore nel caricamento delle categorie per la select:", error);
    }
}

async function caricaClientiSorgenti() {
    try {
        const response = await fetch('/api/clienti');
        const clienti = await response.json();
        const selectCli = document.getElementById('nuovoCliente');

        clienti.forEach(cli => {
            selectCli.innerHTML += `<option value="${cli.id}">${cli.ragioneSociale}</option>`;
        });
    } catch (error) {
        console.error("Errore nel caricamento dei clienti per la select:", error);
    }
}

async function caricaTuttiIDocumenti() {
    try {
        const response = await fetch(API_URL);
        const documento = await response.json();
        renderTabella(documento);
    } catch (error) {
        alert("Errore nel caricamento dei documenti: " + error);
    }
}


async function cercaDocumenti() {
    const nomeCategoria = document.getElementById('filtroCategoria').value.trim();
    const ragioneSociale = document.getElementById('filtroCliente').value.trim();
    const statoValue = document.getElementById('filtroStato').value;

    const params = new URLSearchParams();

    if (nomeCategoria) {
        params.append('categoriaNome', nomeCategoria);
    }

    if (ragioneSociale) {
        params.append('clienteRagioneSociale', ragioneSociale);
    }

    if (statoValue && statoValue !== "Tutti") {
        const statoFormattato = statoValue.charAt(0).toUpperCase() + statoValue.slice(1).toLowerCase();
        params.append('stato', statoFormattato);
    }

    try {
        const response = await fetch(`${API_URL}/ricerca?${params.toString()}`);

        if (!response.ok) {
            throw new Error(`Errore server: Status ${response.status}`);
        }

        const risultati = await response.json();
        renderTabella(risultati);
    } catch (error) {
        console.error("Errore durante la ricerca:", error);
        alert("Errore durante la ricerca filtrata. Controlla la console per i dettagli.");
    }
}
function renderTabella(documenti) {
    const tbody = document.getElementById('tabellaDocumenti');
    tbody.innerHTML = '';

    if (!documenti || documenti.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted py-4">Nessun documento trovato.</td></tr>`;
        return;
    }

    documenti.forEach(doc => {
        let statoBadge = '';
        const statoRaw = doc.stato !== undefined && doc.stato !== null ? doc.stato.toString().trim().toLowerCase() : '';

        switch (statoRaw) {
            case '0':
            case 'bozza':
                statoBadge = '<span class="badge bg-warning text-dark">Bozza</span>';
                break;
            case '1':
            case 'pubblicato':
                statoBadge = '<span class="badge bg-info text-dark">Pubblicato</span>';
                break;
            case '2':
            case 'archiviato':
                statoBadge = '<span class="badge bg-success">Archiviato</span>';
                break;
            case '3':
            case 'annullato':
                statoBadge = '<span class="badge bg-danger">Annullato</span>';
                break;
            default:
                statoBadge = `<span class="badge bg-secondary">${doc.stato || 'Sconosciuto'}</span>`;
                break;
        }

        const fileTesto = doc.nomeFile ? `📄 ${doc.nomeFile}` : '<span class="text-muted small">Nessuno</span>';
        const nomeCategoria = doc.categoriaDocumento && doc.categoriaDocumento.nome ? doc.categoriaDocumento.nome : doc.categoriaId || 'N/D';
        const ragioneSocialeCliente = doc.cliente && doc.cliente.ragioneSociale ? doc.cliente.ragioneSociale : doc.clienteId || 'N/D';

        tbody.innerHTML += `
        <tr>
            <td><strong>#${doc.id}</strong></td>
            <td>${doc.titolo}</td>
            <td><span class="badge bg-secondary">Cat. ${nomeCategoria}</span></td>
            <td><span class="badge bg-info text-dark">Cli. ${ragioneSocialeCliente}</span></td>
            <td>${statoBadge}</td>
            <td>${fileTesto}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-primary" onclick="apriModalAllegato(${doc.id})">📎 Allega File</button>
            </td>
        </tr>
        `;
    });
}

function apriModalAllegato(id) {
    document.getElementById('allegatoDocId').value = id;
    document.getElementById('inputFileElement').value = '';
    document.getElementById('alertModal').classList.add('d-none');
    modalAllegatoIstanza.show();
}

async function inviaAllegatoReale() {
    const id = document.getElementById('allegatoDocId').value;
    const fileInput = document.getElementById('inputFileElement');
    const alertModal = document.getElementById('alertModal');

    alertModal.classList.add('d-none');
    alertModal.textContent = "";

    if (fileInput.files.length === 0) {
        alertModal.textContent = "Seleziona un file prima di procedere!";
        alertModal.classList.remove('d-none');
        return;
    }

    const fileSelezionato = fileInput.files[0];

    if (fileSelezionato.size > 5 * 1024 * 1024) {
        alertModal.textContent = "Errore: Il file supera la dimensione massima di 5MB.";
        alertModal.classList.remove('d-none');
        return;
    }

    const estensioniConsentite = ['.pdf', '.docx', '.doc', '.jpg', '.png'];
    const nomeFile = fileSelezionato.name.toLowerCase();
    const estensioneValida = estensioniConsentite.some(ext => nomeFile.endsWith(ext));

    if (!estensioneValida) {
        alertModal.textContent = "Errore: Estensione non valida. Usa PDF, DOCX, DOC, JPG o PNG.";
        alertModal.classList.remove('d-none');
        return;
    }

    const formData = new FormData();
    formData.append('file', fileSelezionato);

    try {
        const response = await fetch(`${API_URL}/${id}/allegato`, {
            method: 'PATCH',
            body: formData
        });

        const data = await response.json();

        if (response.ok) {
            alert("Allegato salvato con successo!");
            modalAllegatoIstanza.hide();
            caricaTuttiIDocumenti();
        } else {
            alertModal.textContent = data.messaggio || data.errore || "Errore durante il salvataggio.";
            alertModal.classList.remove('d-none');
        }
    } catch (error) {
        console.error("Dettaglio errore server:", error);
        alertModal.textContent = "Errore di connessione con il server.";
        alertModal.classList.remove('d-none');
    }
}

function mostraFormNuovo() {
    document.getElementById('formNuovoDocumento').reset();
    const alertNuovo = document.getElementById('alertNuovoDoc');
    alertNuovo.classList.add('d-none');
    alertNuovo.textContent = "";
    modalNuovoDocIstanza.show();
}

async function creaNuovoDocumento() {
    const alertNuovo = document.getElementById('alertNuovoDoc');
    const titolo = document.getElementById('nuovoTitolo').value.trim();
    const categoriaId = document.getElementById('nuovaCategoria').value.trim();
    const clienteId = document.getElementById('nuovoCliente').value.trim();
    const statoValue = document.getElementById('nuovoStato').value;
    const fileInput = document.getElementById('nuovoFileInput');

    alertNuovo.classList.add('d-none');
    alertNuovo.textContent = "";

    if (!titolo || !categoriaId || !clienteId) {
        alertNuovo.textContent = "Errore: Compila tutti i campi obbligatori (*).";
        alertNuovo.classList.remove('d-none');
        return;
    }

    if (fileInput.files.length === 0) {
        alertNuovo.textContent = "Errore: Devi selezionare obbligatoriamente un file di documento.";
        alertNuovo.classList.remove('d-none');
        return;
    }

    const fileDaCaricare = fileInput.files[0];
    const nomeFileSelezionato = fileDaCaricare.name;

    if (fileDaCaricare.size > 5 * 1024 * 1024) {
        alertNuovo.textContent = "Errore File: Supera la dimensione massima di 5MB.";
        alertNuovo.classList.remove('d-none');
        return;
    }

    const estensioniConsentite = ['.pdf', '.docx', '.doc', '.jpg', '.png'];
    if (!estensioniConsentite.some(ext => nomeFileSelezionato.toLowerCase().endsWith(ext))) {
        alertNuovo.textContent = "Errore File: Estensione ammessa solo PDF, DOCX, DOC, JPG o PNG.";
        alertNuovo.classList.remove('d-none');
        return;
    }

    const statoEnumCorretto = STATO_ENUM_MAP[statoValue] !== undefined ? STATO_ENUM_MAP[statoValue] : 0;

    const payloadDocumento = {
        Titolo: titolo,
        Stato: statoEnumCorretto,
        CategoriaDocumento: { Id: parseInt(categoriaId) },
        Cliente: { Id: parseInt(clienteId) },
        Descrizione: "",
        NomeFile: nomeFileSelezionato
    };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payloadDocumento)
        });

        const data = await response.json();

        if (!response.ok) {
            alertNuovo.textContent = data.messaggio || data.errore || "Errore durante la creazione del record.";
            alertNuovo.classList.remove('d-none');
            return;
        }

        const nuovoIdGenerato = data.id;
        const formDataFile = new FormData();
        formDataFile.append('file', fileDaCaricare);

        const responseFile = await fetch(`${API_URL}/${nuovoIdGenerato}/allegato`, {
            method: 'PATCH',
            body: formDataFile
        });

        if (!responseFile.ok) {
            alert("Record creato, ma il caricamento dell'allegato sul server è fallito.");
            return;
        }

        alert("Documento e file creati con successo!");
        modalNuovoDocIstanza.hide();
        caricaTuttiIDocumenti();

    } catch (error) {
        console.error("Errore server invio POST:", error);
        alertNuovo.textContent = "Errore di rete o server offline.";
        alertNuovo.classList.remove('d-none');
    }
}

async function gestisciNuovaCategoria(selectElement) {
    if (selectElement.value !== "__NEW__") return;

    const nomeNuovaCat = prompt("Inserisci il nome della nuova categoria:");

    if (!nomeNuovaCat || nomeNuovaCat.trim() === "") {
        selectElement.value = "";
        return;
    }

    try {
        const response = await fetch('/api/categorie', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Nome: nomeNuovaCat.trim() })
        });

        if (response.ok) {
            const nuovaCatCreata = await response.json();
            alert("Categoria creata con successo!");

            document.getElementById('nuovaCategoria').innerHTML = `
                <option value="">-- Seleziona una Categoria --</option>
                <option value="__NEW__" class="text-success fw-bold">✨ + Aggiungi Nuova Categoria...</option>
                <option disabled>────────────────────</option>`;
            await caricaCategorieSorgenti();

            document.getElementById('nuovaCategoria').value = nuovaCatCreata.id;
        } else {
            alert("Errore durante la creazione della categoria.");
            selectElement.value = "";
        }
    } catch (error) {
        console.error(error);
        alert("Errore di connessione.");
        selectElement.value = "";
    }
}

async function gestisciNuovoCliente(selectElement) {
    if (selectElement.value !== "__NEW__") return;

    const ragioneSociale = prompt("Inserisci la Ragione Sociale del nuovo cliente:");

    if (!ragioneSociale || ragioneSociale.trim() === "") {
        selectElement.value = "";
        return;
    }

    try {
        const response = await fetch('/api/clienti', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ RagioneSociale: ragioneSociale.trim(), Email: "cliente@temp.it" })
        });

        if (response.ok) {
            const nuovoClienteCreato = await response.json();
            alert("Cliente registrato con successo!");

            document.getElementById('nuovoCliente').innerHTML = `
                <option value="">-- Seleziona un Cliente --</option>
                <option value="__NEW__" class="text-success fw-bold">✨ + Aggiungi Nuovo Cliente...</option>
                <option disabled>────────────────────</option>`;
            await caricaClientiSorgenti();

            document.getElementById('nuovoCliente').value = nuovoClienteCreato.id;
        } else {
            alert("Errore durante la registrazione del cliente.");
            selectElement.value = "";
        }
    } catch (error) {
        console.error(error);
        alert("Errore di connessione.");
        selectElement.value = "";
    }
}