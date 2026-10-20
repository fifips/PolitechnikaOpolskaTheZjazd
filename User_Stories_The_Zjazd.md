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
- Układ lokacji lub ich zawartość różni się między podejściami (element roguelike).

## US-02: Wykonywanie zadań i zdobywanie ECTS

**Jako** student
**chcę** wykonywać zadania i łamigłówki związane z kierunkiem studiów,
**aby** zdobywać punkty ECTS.

**Kryteria akceptacji:**
- Mogę rozpocząć zadanie po interakcji z postacią lub obiektem.
- Za ukończone zadanie otrzymuję punkty ECTS.
- Aktualną liczbę punktów widzę w interfejsie.

## US-03: Przetrwanie zjazdu i energia

**Jako** student
**chcę** zarządzać energią (kawa, energetyki),
**aby** wytrzymać do końca weekendowego zjazdu.

**Kryteria akceptacji:**
- Energia spada wraz z czasem i wykonywanymi działaniami.
- Napoje i jedzenie przywracają energię.
- Gdy energia spadnie do zera, podejście się kończy.

## US-04: Rozwój postaci w trakcie podejścia

**Jako** student
**chcę** rozwijać postać (umiejętności, wyposażenie),
**aby** łatwiej radzić sobie z kolejnymi etapami zjazdu.

**Kryteria akceptacji:**
- Zdobyte ulepszenia działają do końca bieżącego podejścia.
- Ulepszenia widzę w ekwipunku lub na ekranie postaci.
- Po rozpoczęciu nowego podejścia stan postaci jest resetowany (z wyjątkiem trwałych ulepszeń).

## US-05: Ranking graczy

**Jako** gracz
**chcę** zobaczyć swój wynik w globalnym rankingu,
**aby** porównać się z innymi studentami.

**Kryteria akceptacji:**
- Wynik końcowy (np. suma ECTS) trafia do rankingu po zakończeniu podejścia.
- Mogę przeglądać listę najlepszych graczy oraz własną pozycję.
- Tryb treningowy nie wpływa na ranking.


# General User Stories

## US-06

**Jako** gracz
**chcę** w łatwy sposób uruchomić grę,
**aby** rozpocząć granie bez niepotrzebnych przeszkód.

**Kryteria akceptacji:**
- Istnieje jeden (1) plik wykonywalny, który uruchamia grę po podwójnym kliknięciu
- Gra działa na Windows 11
- Gra nie ma żadnych wymagań systemowych poza podstawowymi (system Windows, podłączona klawiatura, etc).

## US-07

**Jako** gracz
**chcę** po uruchomieniu gry mieć widok MENU
**aby** mieć możliwość zapoznania się z opcjami przed rozpoczęciem gry.

**Kryteria akceptacji:**
- po uruchomieniu gry, wyświetlany jest widok MENU.
- Przynajmniej dwie opcje są dostępne: uruchom nową grę oraz zamknij.

## US-08

**Jako** gracz
**chcę** móc w łatwy sposób zatrzymać grę,
**aby** móc przerwać z przyczyn niezależnych.

**Kryteria akceptacji:**
- po wybraniu odpowiedniej opcji, gra zostaje zatrzymana,
- na ekranie pojawia się widok "Wznów grę"
- widok może być zintegrowany z MENU; wtedy MENU będzie pokazywało trzy opcje: Wznów grę, Rozpocznij nową grę, Zakończ.

## US-09

**Jako** gracz
**chcę** mieć możliwość zapisania stanu gry,
**aby** rozpocząć granie od zapisanego momentu.

**Kryteria akceptacji:**
- istnieje przycisk lub kombinacja klawiszy zapisująca stan gry,
- otwierając grę następnym razem, w MENU pojawia się opcja Wznów zapisaną grę
- Jako minimum: ostatni stan jest zapisywany, każdy nowy stan nadpisuje ostatni.

## US-10

**Jako** wydawca gry
**chcę** mieć możliwość wglądu w statystyki graczy
**aby** poznać popularność gry.

**Kryteria akceptacji:**
- gra zapisuje lokalnie plik cache z kumulatywnym czasem spędzonym na graniu.
- gra zapisuje szczegółowe statystyki: ile czasu w budynku, ile czasu na zadaniach, etc.

## US-11

**Jako** Programistka gry
**chcę** mieć możliwość analizy błędów w grze
**aby** móc naprawiać bugi.

**Kryteria akceptacji:**
- przy wystąpieniu nieoczekiwanego błędu, aplikacja zapisuje informacje na temat błędu do pliku diagnostycznego w folderze cache.
- informacja diagnostyczna zawiera stos wywołania oraz komunikat błędu.

