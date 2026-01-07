CarsConnected är ett C#-projekt som hanterar uppkoppling och kommunikation mellan bilar och molntjänster. 
Projektet är utvecklat med .NET 9 och använder objektorienterad programmering för att modellera bilens funktioner och möjliggöra integration med externa tjänster.

Funktioner 
• Uppkoppling av bilar mot molntjänster 
• Kommunikation och dataöverföring mellan bil och server 
• Struktur för vidareutveckling med fler IoT- och bilrelaterade funktioner

Projektstruktur 
• Car.cs – Klass för bilens egenskaper och funktioner 
• ConnectionManager.cs – Hantering av uppkoppling och kommunikation 
• Program.cs – Startpunkt för applikationen 
• CarsConnected.csproj – Projektfil för .NET 9



CarsConnectedAnalysis är ett C#-projekt som hämtar och analyserar sensordata från ThingSpeak för uppkopplade bilar. 
Projektet är utvecklat med .NET 9 och använder objektorienterad programmering för att modellera och hantera bilens sensordata.

Funktioner 
• Hämtning av sensordata (varvtal, hastighet, bränslenivå, motortemperatur) från ThingSpeak API 
• Analys och hantering av insamlad data 
• Struktur för vidareutveckling med fler analysfunktioner

Projektstruktur 
• ReceiveDataThingSpeak.cs – Hämtning av data från ThingSpeak 
• SensorData.cs – Klass för sensordata 
• Program.cs – Startpunkt för applikationen 
• CarsConnectedAnalysis.csproj – Projektfil för .NET 9




Installation 
Klona detta repository: git clone https://github.com/sabinakarlsson/CarsConnected.git 
Öppna projektet i Visual Studio 2022 eller senare. 
Bygg och kör projektet.

Användning 
Starta programmet och följ instruktionerna i konsolen för att hämta och analysera bilens sensordata.

Kontakt Utvecklare: Sabina Karlsson 
GitHub: sabinakarlsson
