# User Stories – The Zjazd

Propozycje user stories dla funkcjonalności głównych gry „The Zjazd".

# Gameplay User Stories

## US-01: Eksploracja kampusu

**Jako** student
**chcę** swobodnie poruszać się po lokacjach inspirowanych budynkami Politechniki Opolskiej,
**aby** szukać zadań, postaci i przedmiotów.

**Kryteria akceptacji:**
- Postacią steruję w widoku z góry 2D.
- Mogę przechodzić między co najmniej kilkoma lokacjami.

## US-02: Urozmaicenie przez losowanie

**Jako** student
**chcę**, aby lokacje i ich zawartość były losowo generowane lub wybierane przy każdym nowym podejściu,
**aby** rozgrywka była bardziej zróżnicowana i mniej przewidywalna (element roguelike).

**Kryteria akceptacji:**
- Przy każdym nowym podejściu układ lub zawartość co najmniej jednej lokacji jest generowana lub wybierana losowo.
- Losowość obejmuje np. rozmieszczenie zadań, przedmiotów lub przeszkód w lokacjach.
- Dwa kolejne podejścia tego samego gracza mogą się różnić pod względem napotkanych lokacji lub zadań.

## US-03: Wykonywanie zadań i zdobywanie ECTS

**Jako** student
**chcę** wykonywać zadania i łamigłówki związane z kierunkiem studiów,
**aby** zdobywać punkty ECTS.

**Kryteria akceptacji:**
- Mogę rozpocząć zadanie po interakcji z postacią lub obiektem.
- Za ukończone zadanie otrzymuję punkty ECTS.
- Aktualną liczbę punktów widzę w interfejsie.

## US-04: Przetrwanie zjazdu i energia

**Jako** student
**chcę** zarządzać energią (kawa, energetyki),
**aby** wytrzymać do końca weekendowego zjazdu.

**Kryteria akceptacji:**
- Energia spada wraz z czasem i wykonywanymi działaniami.
- Napoje i jedzenie przywracają energię.
- Gdy energia spadnie do zera, podejście się kończy.

## US-05: Rozwój postaci w trakcie podejścia

**Jako** student
**chcę** rozwijać postać (umiejętności, wyposażenie),
**aby** łatwiej radzić sobie z kolejnymi etapami zjazdu.

**Kryteria akceptacji:**
- Zdobyte ulepszenia działają do końca bieżącego podejścia.
- Ulepszenia widzę w ekwipunku lub na ekranie postaci.
- Po rozpoczęciu nowego podejścia stan postaci jest resetowany (z wyjątkiem trwałych ulepszeń).

## US-06: Ranking graczy

**Jako** gracz
**chcę** zobaczyć swój wynik w globalnym rankingu,
**aby** porównać się z innymi studentami.

**Kryteria akceptacji:**
- Wynik końcowy (np. suma ECTS) trafia do rankingu po zakończeniu podejścia.
- Mogę przeglądać listę najlepszych graczy oraz własną pozycję.

## US-07: Tryb treningowy

**Jako** gracz
**chcę** mieć dostęp do trybu treningowego bez limitu czasu i energii oraz bez wpływu na ranking,
**aby** swobodnie uczyć się mechanik gry i testować strategie.

**Kryteria akceptacji:**
- W MENU dostępna jest opcja wyboru trybu treningowego, osobna od standardowej rozgrywki.
- W trybie treningowym nie obowiązuje ograniczenie energii/czasu wymuszające zakończenie podejścia.
- Wynik zdobyty w trybie treningowym nie jest zapisywany w globalnym rankingu (US-06).


# General User Stories

## US-08

**Jako** gracz
**chcę** w łatwy sposób uruchomić grę,
**aby** rozpocząć granie bez niepotrzebnych przeszkód.

**Kryteria akceptacji:**
- Istnieje jeden (1) plik wykonywalny, który uruchamia grę po podwójnym kliknięciu
- Gra działa na Windows 11
- Gra nie ma żadnych wymagań systemowych poza podstawowymi (system Windows, podłączona klawiatura, etc).

## US-09

**Jako** gracz
**chcę** po uruchomieniu gry mieć widok MENU
**aby** mieć możliwość zapoznania się z opcjami przed rozpoczęciem gry.

**Kryteria akceptacji:**
- po uruchomieniu gry, wyświetlany jest widok MENU.
- Przynajmniej dwie opcje są dostępne: uruchom nową grę oraz zamknij.

## US-10

**Jako** gracz
**chcę** móc w łatwy sposób zatrzymać grę,
**aby** móc przerwać z przyczyn niezależnych.

**Kryteria akceptacji:**
- po wybraniu odpowiedniej opcji, gra zostaje zatrzymana,
- na ekranie pojawia się widok "Wznów grę"
- widok może być zintegrowany z MENU; wtedy MENU będzie pokazywało trzy opcje: Wznów grę, Rozpocznij nową grę, Zakończ.

## US-11

**Jako** gracz
**chcę** mieć możliwość zapisania stanu gry,
**aby** rozpocząć granie od zapisanego momentu.

**Kryteria akceptacji:**
- istnieje przycisk lub kombinacja klawiszy zapisująca stan gry,
- otwierając grę następnym razem, w MENU pojawia się opcja Wznów zapisaną grę
- Jako minimum: ostatni stan jest zapisywany, każdy nowy stan nadpisuje ostatni.

## US-12

**Jako** wydawca gry
**chcę** mieć możliwość wglądu w statystyki graczy
**aby** poznać popularność gry.

**Kryteria akceptacji:**
- gra zapisuje lokalnie plik cache z kumulatywnym czasem spędzonym na graniu.
- gra zapisuje szczegółowe statystyki: ile czasu w budynku, ile czasu na zadaniach, etc.

## US-13

**Jako** Programistka gry
**chcę** mieć możliwość analizy błędów w grze
**aby** móc naprawiać bugi.

**Kryteria akceptacji:**
- przy wystąpieniu nieoczekiwanego błędu, aplikacja zapisuje informacje na temat błędu do pliku diagnostycznego w folderze cache.
- informacja diagnostyczna zawiera stos wywołania oraz komunikat błędu.

