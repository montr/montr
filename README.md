# [Montr](https://montr.net/) &middot; [![GitHub Actions status](https://github.com/montr/montr/workflows/build/badge.svg)](https://github.com/montr/montr) [![GitHub license](https://img.shields.io/badge/license-AGPL3.0-blue.svg)](https://github.com/montr/montr/blob/master/LICENSE) [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fmontr%2Fmontr.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fmontr%2Fmontr?ref=badge_shield)

R&D of B2B automation applications

* SSO
* MDM
* more to come...

## Table of contents

- 🚀[Getting Started](#getting-started)
- 🧾 [License](#license)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

* [.NET Core 5.0](https://dotnet.microsoft.com/download)
* [Node.js LTS](https://nodejs.org/en/download/)
* [PostgreSQL 12](https://www.postgresql.org/download/)

### Installation

1. Clone repository from `git@github.com:montr/montr.git`
2. Create database `montr` (or choose your database name) in PostgreSQL.
3. Copy sample `secrets.json` from `templates/secrets.json` to `Microsoft/UserSecrets/1f5f8818-a536-4818-b963-2d3ef5dcef03` directory.
   * Specify choosen database name and other connection string parameters in `Default` and `Migration` connections of `ConnectionStrings` section in `secrets.json`.
4. Run dotnet to watch backend sources changes in `./src/Host`. During first startup database structure (tables etc.) and default data (users etc.) will be created.
```bash
dotnet watch run
```
5. Install node packages in `./src/ui`.
```bash
npm install
```
6. Run webpack to watch frontend sources changes in `./src/ui`. Compiled assets will be copied to `./src/Host/wwwroot/assets` and served from these location.
```bash
npm start
```
7. Open http://127.0.0.1:5000 or https://127.0.0.1:5001 in browser.
	* Specify default administrator email and password on opened Setup page.

## License

Montr is [AGPL 3.0 licensed](./LICENSE).
