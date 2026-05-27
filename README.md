# ArchivioDocumenti
# Sistema di Gestione e Archivio Documenti Digitale

Questo progetto consiste in un'applicazione web per la gestione dei documenti aziendali, 
basata su un backend .NET 8 e persistenza dati su SQL Server. 
Il sistema consente il caricamento di file allegati, la categorizzazione e la ricerca flessibile 
dei documenti associati a categorie e clienti.

---

## 1. Configurazione del Database SQL Server

Il sistema si appoggia a un database relazionale composto da tre tabelle principali con vincoli 
di integrità referenziale (`Categorie`, `Clienti`, `Documenti`) e diverse stored procedure 
dedicate alla gestione delle operazioni e alla ricerca testuale avanzata.

Per configurare l'ambiente database prima di avviare l'applicazione:
1. Assicurarsi che l'istanza locale di SQL Server sia attiva e funzionante.
2. Creare un nuovo database vuoto denominato `Archivio`.
3. Eseguire lo script SQL dedicato forniti all'interno della cartella denoninata `Database`.

---

## 2. Configurazione della Connection String

L'applicazione legge i parametri di connessione al database dal file `appsettings.json` 
situato nella cartella principale del progetto di backend. 

Aprire il file e mappare la voce sotto il nodo `ConnectionStrings` adattandola ai parametri 
del proprio server locale:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Archivio;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*"
}

```

---

## 3. Breve nota sulle scelte tecniche

L'architettura del software segue i moderni standard di sviluppo enterprise, 
orientati alla manutenibilità e alle prestazioni:

1. Separazione delle responsabilità (Layered Architecture). Il backend è strutturato in livelli 
   distinti. Questa separazione garantisce che la logica di business sia totalmente indipendente 
   dalle tecnologie di persistenza o dalle interfacce utente.
2. Stored Procedure per la logica dei dati.
3. Ricerca Flessibile (Case & Accent Insensitive)
