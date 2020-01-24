# [Montr](https://montr.net/) &middot; [![GitHub Actions status](https://github.com/montr/montr/workflows/build/badge.svg)](https://github.com/montr/montr) [![GitHub license](https://img.shields.io/badge/license-GPL3.0-blue.svg)](https://github.com/montr/montr/blob/master/LICENSE)

R&D sample of B2B applications

* SSO
* MDM
* more to come...

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

* [.NET Core 3.1](https://dotnet.microsoft.com/download)
* [Node.js LTS](https://nodejs.org/en/download/)
* [PostgreSQL 12](https://www.postgresql.org/download/)

### Installation

* Clone repository from `git@github.com:montr/montr.git`
* Create database `montr-dev` (or choose your database name) in PostgreSQL. Restore sample database backup from `todo`.
* Copy sample `secrets.json` from `todo` to `Microsoft/UserSecrets/1f5f8818-a536-4818-b963-2d3ef5dcef03` directory. Specify choosen database name and other connection string parameters in connection `Default` in section `ConnectionStrings` of `secrets.json`.
* Run dotnet to watch backend sources changes in `./src/Host`
```bash
dotnet watch run
```
* Install node packages in `./src/ui`.
```bash
npm install
```
* Run webpack to watch frontend sources changes in `./src/ui`. Transpiled assets will be copied to `./src/Host/wwwroot/assets` and served from these location.
```bash
npm start
```
* Open http://127.0.0.1:5010 or https://127.0.0.1:5001 in browser.

### License

Montr is [GPL 3.0 licensed](./LICENSE).
