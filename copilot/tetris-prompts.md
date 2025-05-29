# Prompty do Stworzenia Aplikacji Tetris

Poniżej znajduje się lista promptów, które można wykorzystać podczas tworzenia aplikacji Tetris w oparciu o wymagania funkcjonalne i techniczne. Prompty są zorganizowane według etapów rozwoju i komponentów aplikacji.

## Planowanie i Architektura

1. **Plan projektu**
   ```
   Stwórz plan projektu dla aplikacji webowej Tetris w .NET Core 6.0 z C#, uwzględniający podstawowe mechaniki gry (plansza 10x20, 7 standardowych klocków, punktacja), interfejs użytkownika, różne tryby gry i zarządzanie stanem gry.
   ```

2. **Architektura aplikacji**
   ```
   Zaprojektuj architekturę aplikacji webowej Tetris z wykorzystaniem .NET Core 6.0 i C#. Aplikacja powinna mieć responsywny interfejs i wspierać różne przeglądarki. Uwzględnij komponenty do zarządzania stanem gry, rozgrywką, statystykami i ustawieniami użytkownika.
   ```

3. **Model danych**
   ```
   Stwórz model danych SQL dla aplikacji Tetris, który będzie przechowywał informacje o użytkownikach, ich ustawieniach, zapisanych stanach gry, historii rozgrywek i statystykach.
   ```

## Podstawowa Mechanika Gry

4. **Plansza gry**
   ```
   Zaimplementuj klasę Board do gry Tetris w C#, która reprezentuje planszę o wymiarach 10x20 i pozwala na dodawanie, sprawdzanie kolizji i usuwanie pełnych rzędów klocków.
   ```

5. **Klasy tetrominów**
   ```
   Stwórz hierarchię klas dla 7 standardowych klocków Tetris (I, J, L, O, S, T, Z) w C#, z abstrakcyjną klasą bazową Tetromino. Każdy klocek powinien mieć swoją unikalną formę, kolor i logikę obrotu.
   ```

6. **Mechanika spadania**
   ```
   Zaimplementuj mechanikę spadania klocków w grze Tetris, uwzględniającą stałą prędkość spadania, przyspieszenie wraz z postępem gry oraz możliwość przyspieszenia spadania przez gracza.
   ```

7. **Sterowanie klockami**
   ```
   Napisz kod odpowiedzialny za sterowanie klockami w grze Tetris, umożliwiający przesuwanie w lewo i prawo, obracanie zgodnie i przeciwnie do ruchu wskazówek zegara oraz natychmiastowe upuszczanie na dno planszy.
   ```

8. **Usuwanie rzędów i punktacja**
   ```
   Zaimplementuj mechanikę usuwania pełnych rzędów i przyznawania punktów w grze Tetris. Uwzględnij dodatkowe punkty za usuwanie wielu rzędów naraz (podwójne, potrójne, Tetris).
   ```

9. **Warunki końca gry**
   ```
   Zaprogramuj logikę wykrywania końca gry w Tetris, gdy nie ma miejsca na umieszczenie nowego klocka, oraz wyświetlanie odpowiedniego komunikatu z wynikiem.
   ```

## Interfejs Użytkownika

10. **Menu główne**
    ```
    Stwórz responsywny interfejs menu głównego dla gry Tetris, zawierający opcje rozpoczęcia nowej gry, wczytania zapisanej gry i ustawień. Interfejs powinien być intuicyjny i estetyczny.
    ```

11. **Interfejs rozgrywki**
    ```
    Zaprojektuj i zaimplementuj interfejs rozgrywki dla gry Tetris, wyświetlający planszę gry, aktualny wynik, poziom trudności, liczbę usuniętych rzędów oraz podgląd następnego klocka.
    ```

12. **Responsywność interfejsu**
    ```
    Zaimplementuj responsywność interfejsu gry Tetris, aby dostosowywał się do różnych rozmiarów ekranu i zapewniał płynne sterowanie bez opóźnień.
    ```

## Funkcje Rozgrywki

13. **Poziomy trudności**
    ```
    Zaimplementuj system poziomów trudności (łatwy, średni, trudny) dla gry Tetris, wpływający na prędkość spadania klocków i system punktacji.
    ```

14. **Tryby gry**
    ```
    Stwórz zróżnicowane tryby gry dla Tetris: klasyczny, czasowy (zdobyć jak najwięcej punktów w określonym czasie) i wyzwanie (usunąć określoną liczbę rzędów).
    ```

## Zarządzanie Grą

15. **System zapisu i wczytywania gry**
    ```
    Zaprogramuj system zapisu i wczytywania stanu gry Tetris, umożliwiający graczowi powrót do przerwanej rozgrywki.
    ```

16. **System statystyk**
    ```
    Stwórz system zbierający i wyświetlający statystyki gry Tetris, w tym najwyższy wynik, średni wynik i liczbę rozegranych gier, z opcją resetowania statystyk.
    ```

17. **Ustawienia użytkownika**
    ```
    Zaimplementuj system ustawień gry Tetris, pozwalający na dostosowanie sterowania, włączanie/wyłączanie efektów dźwiękowych i muzyki oraz zmianę motywu kolorystycznego.
    ```

## Testowanie i Optymalizacja

18. **Testy jednostkowe**
    ```
    Napisz testy jednostkowe dla kluczowych komponentów gry Tetris, sprawdzające poprawność działania mechaniki gry, kolizji, punktacji i warunków końca gry.
    ```

19. **Testy interfejsu użytkownika**
    ```
    Stwórz testy sprawdzające responsywność i użyteczność interfejsu gry Tetris na różnych urządzeniach i w różnych przeglądarkach.
    ```

20. **Optymalizacja wydajności**
    ```
    Zoptymalizuj aplikację Tetris pod kątem wydajności, aby zapewnić płynną rozgrywkę nawet na słabszych urządzeniach.
    ```

## Wdrożenie i Dokumentacja

21. **Instrukcje wdrożenia**
    ```
    Przygotuj instrukcje wdrożenia aplikacji Tetris, w tym wymagania systemowe, proces instalacji i konfiguracji.
    ```

22. **Dokumentacja użytkownika**
    ```
    Stwórz dokumentację użytkownika dla gry Tetris, opisującą sterowanie, tryby gry, system punktacji i wszystkie dostępne funkcje.
    ```

23. **Dokumentacja dla deweloperów**
    ```
    Przygotuj dokumentację techniczną dla deweloperów, objaśniającą architekturę aplikacji, strukturę kodu i sposób rozszerzania funkcjonalności.
    ```

Te prompty mogą być używane jako punkty wyjścia dla różnych etapów rozwoju aplikacji Tetris i mogą być dostosowywane w zależności od konkretnych potrzeb i wyzwań, które pojawią się podczas implementacji.
