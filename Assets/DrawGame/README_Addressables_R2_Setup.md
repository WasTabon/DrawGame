# Addressables + Cloudflare R2 — Пошаговая настройка

## Шаг 1 — Установка Addressables
1. Window → Package Manager
2. Add package by name: `com.unity.addressables`
3. Версия 1.22.3 или выше

## Шаг 2 — Инициализация Addressables
1. Window → Asset Management → Addressables → Groups
2. Если появится кнопка "Create Addressables Settings" — нажми её
3. Появится дефолтная группа "Default Local Group"

## Шаг 3 — Создание группы Music
1. В окне Addressables Groups: Create → Group → Packed Assets
2. Переименуй группу в `Music`

## Шаг 4 — Настройка группы Music на Remote
1. Выбери группу `Music` в списке
2. В Inspector справа:
   - Build Path → выбери `RemoteBuildPath`
   - Load Path → выбери `RemoteLoadPath`

## Шаг 5 — Настройка профиля
1. Window → Asset Management → Addressables → Profiles
2. В строке `Remote.BuildPath` установи: `ServerData/[BuildTarget]`
3. В строке `Remote.LoadPath` установи: `https://pub-9ec825916e0a4cf0a10e995611990f24.r2.dev/DrawGame/[BuildTarget]`

## Шаг 6 — Включение Remote Catalog
1. Window → Asset Management → Addressables → Settings (или выбери AddressableAssetSettings в Project)
2. Найди раздел Catalog
3. Включи **Build Remote Catalog** (галочка)
4. Build Path → `RemoteBuildPath`
5. Load Path → `RemoteLoadPath`

## Шаг 7 — Добавление музыки
1. Положи .wav файл в любую папку проекта (например `Assets/DrawGame/Audio/`)
2. Перетащи этот файл в группу `Music` в окне Addressables Groups
3. В колонке "Addressable Name" измени имя на: `GameMusic`
4. Правый клик на ассет в группе → выбери "Labels" → создай лейбл `music` → поставь галочку

## Шаг 8 — Билд Addressables
1. В окне Addressables Groups: Build → New Build → Default Build Script
2. Дождись завершения

## Шаг 9 — Проверка сгенерированных файлов
После билда в корне проекта появится папка:
```
ProjectRoot/
  ServerData/
    iOS/
      catalog_1.0.hash
      catalog_1.0.json
      music_assets_all_ХХХХХХ.bundle
```
Должно быть 3 файла. Если bundle нет — музыка не добавлена в группу (вернись к шагу 7).

## Шаг 10 — Заливка в Cloudflare R2
1. Открой Cloudflare Dashboard → R2 → бакет `draw-game`
2. Создай папку `DrawGame`
3. Внутри `DrawGame` создай папку `iOS`
4. Загрузи все 3 файла из `ServerData/iOS/` в `DrawGame/iOS/`

Итоговая структура в бакете:
```
draw-game/                          ← бакет
  DrawGame/
    iOS/
      catalog_1.0.hash
      catalog_1.0.json
      music_assets_all_ХХХХХХ.bundle
```

## Шаг 11 — Проверка доступности
Открой в браузере:
```
https://pub-9ec825916e0a4cf0a10e995611990f24.r2.dev/DrawGame/iOS/catalog_1.0.json
```
Если видишь JSON — всё работает.

## Шаг 12 — Тестирование в Unity
1. На объекте AddressableLoader в Bootstrap сцене сними галочку `Skip Download In Editor`
2. Нажми Play
3. Должен пойти процесс: Initializing → Checking for updates → Downloading → Loading → Ready → переход на MainMenu
4. Музыка должна заиграть

## При обновлении музыки
1. Замени .wav файл
2. Addressables Groups → Build → New Build → Default Build Script
3. Залей новые файлы из `ServerData/iOS/` в R2 (перезапиши старые)
