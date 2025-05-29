# Tetris - Wymagania Funkcjonalne

## Epiki

### EP01: Podstawowa mechanika gry
Implementacja podstawowych mechanizmów gry Tetris, obejmujących planszę gry, spadające klocki i mechanikę punktacji.

### EP02: Interfejs użytkownika
Stworzenie intuicyjnego i responsywnego interfejsu użytkownika dla gry.

### EP03: Funkcje rozgrywki
Dodatkowe funkcje urozmaicające rozgrywkę, takie jak poziomy trudności, tryby gry itp.

### EP04: Zarządzanie grą
Funkcje zarządzania grą, takie jak zapis/wczytanie stanu gry, statystyki i ustawienia.

## Funkcje i historie użytkownika

### EP01: Podstawowa mechanika gry

#### F01-01: Plansza gry
- **US01-01-01**: Jako gracz, chcę widzieć planszę gry o standardowych wymiarach (10x20), aby móc grać w Tetris.
- **US01-01-02**: Jako gracz, chcę, aby plansza gry była wyraźnie widoczna i odgraniczona od reszty interfejsu.

#### F01-02: Klocki Tetris (tetromina)
- **US01-02-01**: Jako gracz, chcę mieć dostęp do wszystkich 7 standardowych klocków Tetris (I, J, L, O, S, T, Z).
- **US01-02-02**: Jako gracz, chcę, aby klocki były kolorowe i łatwe do rozróżnienia.
- **US01-02-03**: Jako gracz, chcę widzieć podgląd następnego klocka, który pojawi się na planszy.

#### F01-03: Mechanika spadania klocków
- **US01-03-01**: Jako gracz, chcę, aby klocki spadały z góry planszy z określoną prędkością.
- **US01-03-02**: Jako gracz, chcę, aby prędkość spadania klocków zwiększała się wraz z postępem gry.
- **US01-03-03**: Jako gracz, chcę móc przyspieszać spadanie klocków, gdy zdecyduję się na to.

#### F01-04: Sterowanie klockami
- **US01-04-01**: Jako gracz, chcę móc przesuwać spadające klocki w lewo i prawo.
- **US01-04-02**: Jako gracz, chcę móc obracać spadające klocki zgodnie i przeciwnie do ruchu wskazówek zegara.
- **US01-04-03**: Jako gracz, chcę móc natychmiast upuścić klocek na dno planszy.

#### F01-05: Usuwanie rzędów i punktacja
- **US01-05-01**: Jako gracz, chcę, aby pełne rzędy były usuwane z planszy.
- **US01-05-02**: Jako gracz, chcę otrzymywać punkty za usuwanie rzędów.
- **US01-05-03**: Jako gracz, chcę otrzymywać więcej punktów za usuwanie wielu rzędów naraz (np. podwójne, potrójne, Tetris).

#### F01-06: Koniec gry
- **US01-06-01**: Jako gracz, chcę, aby gra kończyła się, gdy nie ma miejsca na umieszczenie nowego klocka.
- **US01-06-02**: Jako gracz, chcę widzieć wyraźny komunikat o końcu gry wraz z moim wynikiem.

### EP02: Interfejs użytkownika

#### F02-01: Menu główne
- **US02-01-01**: Jako gracz, chcę mieć dostęp do menu głównego z opcjami rozpoczęcia nowej gry, wczytania zapisanej gry i ustawień.
- **US02-01-02**: Jako gracz, chcę, aby menu było intuicyjne i estetyczne.

#### F02-02: Informacje w trakcie gry
- **US02-02-01**: Jako gracz, chcę widzieć aktualny wynik podczas gry.
- **US02-02-02**: Jako gracz, chcę widzieć aktualny poziom trudności podczas gry.
- **US02-02-03**: Jako gracz, chcę widzieć liczbę usuniętych rzędów podczas gry.

#### F02-03: Responsywność interfejsu
- **US02-03-01**: Jako gracz, chcę, aby interfejs gry był responsywny i dostosowywał się do różnych rozmiarów ekranu.
- **US02-03-02**: Jako gracz, chcę, aby sterowanie działało płynnie i bez opóźnień.

### EP03: Funkcje rozgrywki

#### F03-01: Poziomy trudności
- **US03-01-01**: Jako gracz, chcę móc wybierać poziom trudności przed rozpoczęciem gry.
- **US03-01-02**: Jako gracz, chcę, aby poziom trudności wpływał na prędkość spadania klocków i system punktacji.

#### F03-02: Tryby gry
- **US03-02-01**: Jako gracz, chcę mieć dostęp do klasycznego trybu gry Tetris.
- **US03-02-02**: Jako gracz, chcę mieć dostęp do trybu czasowego, gdzie muszę zdobyć jak najwięcej punktów w określonym czasie.
- **US03-02-03**: Jako gracz, chcę mieć dostęp do trybu wyzwania, gdzie muszę usunąć określoną liczbę rzędów.

### EP04: Zarządzanie grą

#### F04-01: Zapis i wczytywanie gry
- **US04-01-01**: Jako gracz, chcę móc zapisać stan gry, aby móc wrócić do niej później.
- **US04-01-02**: Jako gracz, chcę móc wczytać zapisaną grę i kontynuować od miejsca, w którym skończyłem.

#### F04-02: Statystyki
- **US04-02-01**: Jako gracz, chcę widzieć statystyki moich gier (najwyższy wynik, średni wynik, liczba rozegranych gier).
- **US04-02-02**: Jako gracz, chcę móc resetować moje statystyki, jeśli zechcę.

#### F04-03: Ustawienia
- **US04-03-01**: Jako gracz, chcę móc dostosować sterowanie do moich preferencji.
- **US04-03-02**: Jako gracz, chcę móc włączać/wyłączać efekty dźwiękowe i muzykę.
- **US04-03-03**: Jako gracz, chcę móc dostosować wygląd gry (np. motyw kolorystyczny).

## Wymagania techniczne
- Aplikacja zostanie zbudowana przy użyciu .Net Core 6.0 lub nowszego
- Język programowania: C#
- Aplikacja będzie działać jako aplikacja webowa
- Interfejs użytkownika powinien być responsywny i kompatybilny z popularnymi przeglądarkami
