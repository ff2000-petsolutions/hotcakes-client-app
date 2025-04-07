# Branch stratégia – Hotcakes Client App

Ez a dokumentum leírja, hogyan dolgozunk a repóban:

## Ágak (Branches)
- `main` – csak tesztelt, végleges kiadások (éles build)
- `dev` – fejlesztésre kész, de még nem kiadott állapot
- `feature/...` – minden új fejlesztés külön ágon történik

## Fejlesztési folyamat

1. Új feladat → új branch: `feature/...`
2. Fejlesztés, majd belső tesztelés
3. Teszt után **pull request a `dev` branch-re**
4. Ha minden OK, végül `dev` → `main` megy

## Példák branch nevekre
- `feature/login`
- `feature/cart-ui`
- `refactor/api-client`

## Release
A `main` branch minden commitja külön verzió:
- verziócímkék: `v1.0.0`, `v1.1.0`
- build fájlok: a `releases/` mappába kerülnek
