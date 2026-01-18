# Reflektion – BudgetTracker (Ahmed)

## Inledning
Projektet **BudgetTracker** utvecklades som en .NET-baserad applikation för budget- och ekonomihantering. Arbetet bedrevs enligt **Test-Driven Development (TDD)** och med tydlig lagerindelning.  
I projektet ansvarade **Rayan** för Core, **Stephen** för Api och **jag (Ahmed)** för **BudgetTracker.Web**.

Denna reflektion fokuserar på mitt arbete, de problem jag stötte på, hur vi använde Git och vad jag lärt mig tekniskt och i grupparbete.

---

## Mitt ansvarsområde – BudgetTracker.Web
Mitt ansvar var att utveckla webbgränssnittet som konsumerar API:t och presenterar data på ett tydligt sätt för användaren. Fokus låg på:
- kommunikation med API:t via HttpClient,
- korrekt datahantering och presentation,
- att webblagret förblev tunt och inte innehöll affärslogik.

---

## Faktiska kodexempel från projektet

### Exempel: Anrop till API från Web-lagret
Ett vanligt mönster i **BudgetTracker.Web** var att anropa API:t via `HttpClient` för att hämta data:

```csharp
public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync()
{
    var response = await _httpClient.GetAsync("api/transactions");

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>() 
           ?? Enumerable.Empty<TransactionDto>();
}
```

**Förklaring:**
- Web-lagret innehåller ingen affärslogik, endast kommunikation.
- `EnsureSuccessStatusCode()` används för att snabbt fånga API-fel.
- DTO:er används för att undvika direkt beroende till Core-modeller.
- Detta gjorde koden testbar och tydligt separerad från backend-logik.

### Exempel: Presentation i Web (Razor)
I Web-delen fokuserade jag på enkel och tydlig presentation:

```csharp
@foreach (var transaction in Model.Transactions)
{
    <tr>
        <td>@transaction.Date</td>
        <td>@transaction.Category</td>
        <td>@transaction.Amount</td>
    </tr>
}
```

**Varför detta är viktigt:**
- All logik sker före rendering.
- View-filer hålls enkla och läsbara.
- Mindre risk för buggar i UI-lagret.

---

## Konkreta problem jag stötte på

### Problem 1: CI-fel på grund av merge conflicts
Ett stort problem uppstod när CI-builden misslyckades med felet:

> **Merge conflict marker encountered (CS8300)**

**Orsak:**
- Merge conflict-markeringar (`<<<<<<<`, `=======`, `>>>>>>>`) hade av misstag committats.
- Det fanns dessutom en nästlad kopia av repot som skapade förvirring kring vilken fil som byggdes.

**Lösning:**
- Systematisk felsökning med `git grep "<<<<<<<"`.
- Identifiering och borttagning av ett felaktigt submodule/gitlink.
- Rensning av repo-strukturen.

**Lärdom:**
- CI bygger alltid det som är committat – inte det som ser rätt ut lokalt.
- Strukturproblem i Git kan skapa extremt svårfelsökta fel.

### Problem 2: Spårade build-artefakter (bin/obj)
Vid arbete i Web-projektet uppstod problem när `git add .` misslyckades p.g.a. låsta filer.

**Orsak:**
- `bin/`, `obj/` och `.vs` var spårade i Git.

**Lösning:**
Korrekt `.gitignore`:

```gitignore
.vs/
**/bin/
**/obj/
```

Samt borttagning av artefakter från Git-index.

**Lärdom:**
- En korrekt `.gitignore` är avgörande för ett stabilt samarbete.
- Små Git-misstag kan blockera hela teamet.

---

## Git-reflektion

### Hur vi använde branches
- Vi arbetade i separata branches kopplade till våra ansvarsområden.
- `develop` användes som integrationsbranch.
- Feature-arbete skedde i egna branches innan merge.

### Utmaningar
- Merge conflicts p.g.a. parallella ändringar.
- Svårigheter att identifiera rätt fil vid konflikt p.g.a. fel repo-struktur.

### Vad jag lärde mig
Att alltid:
- göra `git status` och `git diff` före commit,
- köra `git grep "<<<<<<<"` före push,
- förstå skillnaden mellan *working tree* och *index*.

### Vad jag skulle gjort annorlunda
- Satt upp tydligare Git-regler tidigt.
- Säkerställt `.gitignore` redan dag 1.
- Mindre “snabba fixes” direkt på `develop`.

---

## Fördjupad grupparbetsreflektion

### Konkreta utmaningar
- Parallellt arbete i olika lager skapade beroenden.
- Problem i ett lager påverkade hela systemet (ex: API → Web).

### Hur vi hanterade dem
- Tydligare kommunikation om ändringar.
- Snabb feedback när något bröt integrationen.
- Gemensam felsökning vid CI-problem.

### Lärdomar om kommunikation
- Små förändringar bör kommuniceras direkt.
- Det är bättre att fråga tidigt än att lösa allt själv.
- Gemensam förståelse är viktigare än individuellt tempo.

---

## Tekniska detaljer & förbättringsförslag

### Cache & TTL (förslag)
Ett möjligt förbättringsområde är caching i API/Web:
- **TTL-förslag:** 5–10 minuter för läsdata (ex. kategorier)
- **Motivering:** Data ändras sällan men används ofta.

**Trade-off:**
- För kort TTL → fler API-anrop
- För lång TTL → risk för inaktuell data

### Förbättringsförslag
- Introducera retry-policy (t.ex. Polly) vid API-anrop.
- Bättre felmeddelanden i Web UI.
- Mer end-to-end tester mellan Web och API.

---

## Slutsats
Arbetet med **BudgetTracker.Web** gav mig djupare förståelse för:
- hur frontend och backend samverkar i .NET,
- vikten av tydlig arkitektur och ansvarsfördelning,
- hur Git och CI påverkar hela teamets effektivitet.

De problem som uppstod var krävande men mycket lärorika. Projektet stärkte både mina tekniska färdigheter och min förmåga att arbeta strukturerat i grupp, vilket gör detta till ett av de mest utvecklande projekten jag genomfört.
