# The Zjazd

Gra 2D z widokiem z góry, osadzona na kampusie Politechniki Opolskiej. Gracz jako student zaoczny przetrwa weekendowy zjazd: wykonuje zadania, zdobywa punkty ECTS i zarządza energią. Opis funkcji: [User_Stories_The_Zjazd.md](User_Stories_The_Zjazd.md).

## Wymagania

- **Unity 6000.3.28f1** (LTS, dokładnie ta wersja, wszyscy używamy tej samej).
- Unity Hub z modułem **Windows Build Support (Mono)**.
- Licencja Unity Personal.

## Jak otworzyć projekt

1. Sklonuj repozytorium: `git clone https://github.com/fifips/PolitechnikaOpolskaTheZjazd.git`
2. W Unity Hub: **Projects → Add → Add project from disk** i wskaż folder **`PolitechnikaOpolskaTheZjazd/`** wewnątrz sklonowanego repozytorium (nie sam root: projekt Unity jest w podkatalogu, a w roocie są tylko dokumenty i prezentacje).
3. Hub zaproponuje wersję edytora z `PolitechnikaOpolskaTheZjazd/ProjectSettings/ProjectVersion.txt`. Jeśli jej nie masz, zainstaluj `6000.3.28f1`.
4. Pierwsze otwarcie trwa kilka minut (Unity generuje folder `Library/`, którego nie commitujemy).
5. Nie zgadzaj się na "upgrade" projektu do innej wersji edytora.
6. Ustaw platformę docelową: **File → Build Profiles** (lub Build Settings) → **Windows** → **Switch Platform**. Unity trzyma ten wybór lokalnie (w `Library/`), więc nie trafia do repozytorium i każdy ustawia go u siebie. Gra docelowo działa na Windows 11 (jeden plik `.exe`).

## Pliki `.sln` i `.csproj`

Plików `.sln`, `.slnx` i `.csproj` nie ma w repozytorium. Unity generuje je samo przy otwarciu projektu i różnią się między komputerami (ścieżki do edytora, system operacyjny), więc ich nie commitujemy.

Jeśli IDE (Rider, Visual Studio, VS Code) nie widzi skryptów albo nie podpowiada kodu:

1. W Unity: **Edit → Preferences → External Tools** (na macOS: **Unity → Settings → External Tools**).
2. Wybierz swoje IDE w polu **External Script Editor**.
3. Kliknij **Regenerate project files**.
4. Otwórz skrypt dwuklikiem w Unity albo ponownie otwórz solution w IDE.

Wybór IDE jest lokalny i nie trafia do repozytorium.

## Struktura folderów

Projekt Unity leży w `PolitechnikaOpolskaTheZjazd/`; poniższe ścieżki są względem tego folderu.

| Folder | Zawartość |
|---|---|
| `Assets/Scenes` | sceny gry (na razie `SampleScene`) |
| `Assets/Scripts` | skrypty C# |
| `Assets/Prefabs` | prefaby (postać, budynki, zadania) |
| `Assets/Sprites` | grafiki i animacje |
| `Assets/Tiles` | kafelki mapy |

## Konwencje pracy

- Do `main` tylko przez Pull Request, z co najmniej jedną recenzją.
- Nie edytujemy jednej wspólnej sceny równolegle: każdy pracuje na osobnych prefabach, a scenę zmienia tylko po uzgodnieniu, żeby uniknąć konfliktów.
- Pliki `.meta` commitujemy razem z zasobami (nie dodajemy ich do `.gitignore`).
- Nie commitujemy `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `Build/`.
