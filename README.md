# The Zjazd

Gra 2D z widokiem z góry, osadzona na kampusie Politechniki Opolskiej. Gracz jako student zaoczny przetrwa weekendowy zjazd: wykonuje zadania, zdobywa punkty ECTS i zarządza energią. Opis funkcji: [User_Stories_The_Zjazd.md](User_Stories_The_Zjazd.md).

## Wymagania

- **Unity 6000.3.28f1** (LTS, dokładnie ta wersja, wszyscy używamy tej samej).
- Unity Hub z modułem **Windows Build Support (Mono)**.
- Licencja Unity Personal.

## Jak otworzyć projekt

1. Sklonuj repozytorium: `git clone https://github.com/fifips/PolitechnikaOpolskaTheZjazd.git`
2. W Unity Hub: **Projects → Add → Add project from disk** i wskaż sklonowany folder.
3. Hub zaproponuje wersję edytora z `ProjectSettings/ProjectVersion.txt`. Jeśli jej nie masz, zainstaluj `6000.3.28f1`.
4. Pierwsze otwarcie trwa kilka minut (Unity generuje folder `Library/`, którego nie commitujemy).
5. Nie zgadzaj się na "upgrade" projektu do innej wersji edytora.

## Struktura folderów

| Folder | Zawartość |
|---|---|
| `Assets/Scenes` | sceny gry (start: `World`) |
| `Assets/Scripts` | skrypty C# |
| `Assets/Prefabs` | prefaby (postać, budynki, zadania) |
| `Assets/Sprites` | grafiki i animacje |
| `Assets/Tiles` | kafelki mapy |

## Konwencje pracy

- Do `main` tylko przez Pull Request, z co najmniej jedną recenzją.
- Nie edytujemy jednej wspólnej sceny równolegle: każdy pracuje na osobnych prefabach, a scenę zmienia tylko po uzgodnieniu, żeby uniknąć konfliktów.
- Pliki `.meta` commitujemy razem z zasobami (nie dodajemy ich do `.gitignore`).
- Nie commitujemy `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `Build/`.
