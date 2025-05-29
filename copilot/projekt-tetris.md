# Plan projektu dla aplikacji webowej Tetris w .NET Core 6.0

## 1. Przegląd projektu

### Cel projektu
Stworzenie w pełni funkcjonalnej aplikacji webowej Tetris wykorzystującej .NET Core 6.0 i C#, która zaoferuje klasyczną rozgrywkę Tetris z różnymi trybami gry, systemem punktacji i przyjaznym interfejsem użytkownika.

### Główne funkcjonalności
- Podstawowa mechanika gry Tetris (plansza 10x20, 7 standardowych bloków)
- Responsywny interfejs użytkownika działający na różnych urządzeniach
- Różne tryby gry (klasyczny, czasowy, wyzwanie)
- System punktacji i statystyk
- Zarządzanie stanem gry (zapis/wczytywanie)
- Ustawienia użytkownika

## 2. Specyfikacja techniczna

### Platforma i technologie
- Backend: .NET Core 6.0, C#, ASP.NET Core
- Frontend: HTML5, CSS3, JavaScript, Blazor WebAssembly
- Baza danych: Entity Framework Core z bazą SQL Server/SQLite
- Testy: xUnit, Selenium
- Deployment: Docker, Microsoft Azure

## 3. Architektura aplikacji

### Warstwy aplikacji
1. **Warstwa interfejsu użytkownika (UI)**
   - Komponenty Blazor do renderowania gry i interfejsu
   - JavaScript do obsługi zdarzeń klawiatury i ekranu dotykowego

2. **Warstwa logiki biznesowej**
   - Silnik gry Tetris
   - Zarządzanie stanem gry
   - System punktacji

3. **Warstwa dostępu do danych**
   - Repozytoria Entity Framework
   - Model danych

4. **Warstwa komunikacji**
   - API do obsługi komunikacji między frontendem i backendem
   - Obsługa SignalR dla komunikacji w czasie rzeczywistym (opcjonalnie)

### Komponenty systemu
1. **TetrisGame** - główny silnik gry
2. **BoardManager** - zarządzanie planszą gry
3. **TetrominoManager** - logika bloków Tetris
4. **ScoreManager** - system punktacji
5. **GameStateManager** - zarządzanie stanem gry
6. **UserManager** - zarządzanie użytkownikami i ustawieniami
7. **StatisticsManager** - zarządzanie statystykami

## 4. Model danych

### Encje
1. **User**
   - UserId (PK)
   - Username
   - Email
   - PasswordHash
   - RegistrationDate

2. **UserSettings**
   - SettingsId (PK)
   - UserId (FK)
   - ControlSettings
   - SoundEnabled
   - MusicEnabled
   - ColorTheme

3. **GameState**
   - GameStateId (PK)
   - UserId (FK)
   - BoardState (JSON)
   - CurrentScore
   - CurrentLevel
   - NextTetromino
   - SaveDate

4. **GameStatistics**
   - StatisticsId (PK)
   - UserId (FK)
   - HighestScore
   - AverageScore
   - TotalGamesPlayed
   - TotalRowsCleared
   - TotalTimePlayed

5. **GameHistory**
   - GameHistoryId (PK)
   - UserId (FK)
   - GameDate
   - Score
   - Level
   - Duration
   - GameMode

## 5. Szczegółowy plan implementacji

### Etap 1: Podstawowa mechanika gry
1. **Implementacja klasy Board**
   - Reprezentacja planszy 10x20
   - Logika dodawania bloków
   - Wykrywanie kolizji
   - Usuwanie pełnych wierszy

2. **Implementacja klas Tetromino**
   - Abstrakcyjna klasa bazowa Tetromino
   - 7 klas dla standardowych bloków (I, J, L, O, S, T, Z)
   - Logika kształtów, kolorów i rotacji

3. **Implementacja mechaniki spadania**
   - Stała prędkość spadania
   - Akceleracja wraz z postępem gry
   - Przyspieszanie przez gracza

4. **Implementacja sterowania**
   - Ruch w prawo/lewo
   - Rotacja zgodnie/przeciwnie do ruchu wskazówek zegara
   - Natychmiastowy spadek na dół

5. **Implementacja czyszczenia wierszy i punktacji**
   - Wykrywanie pełnych wierszy
   - Usuwanie pełnych wierszy
   - System punktacji z premiami za wielokrotne wiersze

6. **Implementacja warunków końca gry**
   - Detekcja braku miejsca na nowy blok
   - Wyświetlanie komunikatu końcowego z wynikiem

### Etap 2: Interfejs użytkownika
1. **Implementacja głównego menu**
   - Nowa gra
   - Wczytaj grę
   - Ustawienia
   - Statystyki
   - Instrukcje

2. **Implementacja interfejsu rozgrywki**
   - Wyświetlanie planszy gry
   - Panel z wynikiem, poziomem i licznikiem wierszy
   - Podgląd następnego bloku
   - Przyciski sterowania (dla urządzeń mobilnych)

3. **Implementacja responsywności interfejsu**
   - Dostosowanie do różnych rozmiarów ekranów
   - Płynna kontrola na różnych urządzeniach

### Etap 3: Tryby gry i funkcje dodatkowe
1. **Implementacja poziomów trudności**
   - Łatwy (wolne spadanie)
   - Średni (standardowe spadanie)
   - Trudny (szybkie spadanie)

2. **Implementacja trybów gry**
   - Klasyczny (do końca gry)
   - Czasowy (maksymalizacja punktów w określonym czasie)
   - Wyzwanie (czyszczenie określonej liczby wierszy)

3. **Implementacja systemu zapisu/wczytywania**
   - Zapis stanu gry do bazy danych
   - Wczytywanie zapisanego stanu

4. **Implementacja systemu statystyk**
   - Zbieranie danych o rozgrywce
   - Wyświetlanie statystyk (najwyższe wyniki, średnie, czas gry)

5. **Implementacja ustawień użytkownika**
   - Konfiguracja sterowania
   - Włączanie/wyłączanie dźwięków i muzyki
   - Zmiana motywu kolorystycznego

### Etap 4: Testy i optymalizacja
1. **Testy jednostkowe**
   - Testy mechaniki gry
   - Testy wykrywania kolizji
   - Testy punktacji
   - Testy warunków końca gry

2. **Testy interfejsu**
   - Testy responsywności
   - Testy użyteczności na różnych urządzeniach
   - Testy wydajności

3. **Optymalizacja wydajności**
   - Optymalizacja renderowania
   - Optymalizacja logiki gry
   - Zmniejszenie wykorzystania zasobów

### Etap 5: Deployment i dokumentacja
1. **Przygotowanie do wdrożenia**
   - Konfiguracja Docker
   - Przygotowanie środowiska Azure

2. **Dokumentacja użytkownika**
   - Instrukcje gry
   - Opis trybów gry i punktacji
   - FAQ

3. **Dokumentacja techniczna**
   - Opis architektury
   - Opis klas i interfejsów
   - Instrukcje rozbudowy funkcjonalności

## 6. Harmonogram projektu

| Etap | Zadanie | Czas trwania | Zależności |
|------|---------|--------------|------------|
| 1 | Podstawowa mechanika gry | 3 tygodnie | - |
| 2 | Interfejs użytkownika | 2 tygodnie | Etap 1 |
| 3 | Tryby gry i funkcje dodatkowe | 3 tygodnie | Etap 1, Etap 2 |
| 4 | Testy i optymalizacja | 2 tygodnie | Etap 1, Etap 2, Etap 3 |
| 5 | Deployment i dokumentacja | 1 tydzień | Etap 1, Etap 2, Etap 3, Etap 4 |

**Całkowity szacowany czas realizacji: 11 tygodni**

## 7. Ryzyka projektu i strategie mitygacji

| Ryzyko | Wpływ | Prawdopodobieństwo | Strategia mitygacji |
|--------|-------|-------------------|---------------------|
| Problemy z wydajnością na urządzeniach mobilnych | Wysoki | Średnie | Testowanie na różnych urządzeniach, optymalizacja kodu |
| Trudności z implementacją responsywności | Średni | Wysokie | Wykorzystanie frameworków CSS, testowanie na różnych rozmiarach ekranu |
| Problemy z obsługą różnych przeglądarek | Średni | Średnie | Testowanie kross-przeglądarkowe, użycie autoprefixerów |
| Opóźnienia w harmonogramie | Wysoki | Średnie | Regularne przeglądy postępu, priorytetyzacja funkcjonalności |
| Problemy z wydajnością bazy danych | Średni | Niskie | Optymalizacja zapytań, indeksowanie, monitoring |

## 8. Kamienie milowe

1. **Milestone 1**: Działająca podstawowa mechanika gry (koniec Etapu 1)
2. **Milestone 2**: Kompletny interfejs użytkownika (koniec Etapu 2)
3. **Milestone 3**: Implementacja wszystkich trybów gry (środek Etapu 3)
4. **Milestone 4**: Kompletny system zapisu/wczytywania i statystyk (koniec Etapu 3)
5. **Milestone 5**: Zakończone testy i optymalizacja (koniec Etapu 4)
6. **Milestone 6**: Aplikacja gotowa do wdrożenia (koniec Etapu 5)
