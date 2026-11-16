# C# Web, API and Security Test Framework

Web UI, REST API and security test automation in C#, using Selenium WebDriver, NUnit, RestSharp and OWASP ZAP, against a deliberately vulnerable application running locally.

> **Status:** project skeleton. Tests are added one backlog item at a time; see the roadmap below.

## Rules of engagement

This project tests **only** [OWASP Juice Shop](https://owasp.org/www-project-juice-shop/) running on your own machine. Juice Shop is intentionally insecure and built for security training.

Never point these tests or the ZAP scanner at a site or system you don't own or don't have written permission to test.

## What this tests

| Layer | Tooling | What it covers |
|---|---|---|
| Web UI | Selenium WebDriver, NUnit | Login, search, basket and checkout flows through page objects |
| API | RestSharp, NUnit | REST endpoints: CRUD behaviour, status codes, response structure |
| Security | NUnit, OWASP ZAP | Authentication and authorization flaws, input handling, security headers, automated baseline scan |

## Tech stack

C# · .NET 10 · Selenium WebDriver 4 · NUnit · RestSharp · OWASP ZAP · Docker · GitHub Actions

## Prerequisites

- **.NET 10 SDK.** Check with `dotnet --version`.
- **Docker Desktop**, running. Check with `docker --version`.
- **Google Chrome.** Selenium downloads the matching driver automatically.
- **Git.**

## Setup

**1. Clone the repository**

```bash
git clone https://github.com/theguptajii07/<repo-name>.git
cd <repo-name>
```

**2. Start the target application**

Create a Docker network so the scanner can reach the app later, then start Juice Shop on it:

```bash
docker network create qa-net
docker run -d --name juice-shop --network qa-net -p 3000:3000 bkimminich/juice-shop
```

Open http://localhost:3000 in your browser to confirm it's running. It can take 20 to 30 seconds to start the first time.

**3. Restore and build**

```bash
dotnet restore
dotnet build
```

**4. Run the tests**

```bash
dotnet test
```

At this stage there may be no tests yet. A successful build with zero tests means the setup works.

## Running a subset

Tests are tagged with NUnit categories:

```bash
dotnet test --filter "TestCategory=Web"
dotnet test --filter "TestCategory=Api"
dotnet test --filter "TestCategory=Security"
```

## Configuration

Settings live in `appsettings.json` and can be overridden with environment variables, so nothing environment-specific is hardcoded.

| Setting | Default | Purpose |
|---|---|---|
| `BaseUrl` | `http://localhost:3000` | Address of the application under test |
| `Headless` | `true` | Run Chrome without a visible window |
| `DefaultTimeoutSeconds` | `10` | Explicit wait timeout for Selenium |

Credentials for test accounts are never committed. Copy `.env.example` to `.env` and fill in your own values.

## Security scan

The ZAP baseline scan crawls the app and reports passive findings, such as missing security headers and insecure cookies. It doesn't actively attack anything.

PowerShell:

```powershell
docker run --rm --network qa-net -v "${PWD}/reports:/zap/wrk:rw" -t ghcr.io/zaproxy/zaproxy:stable zap-baseline.py -t http://juice-shop:3000 -r zap-baseline.html
```

macOS or Linux:

```bash
docker run --rm --network qa-net -v "$(pwd)/reports:/zap/wrk:rw" -t ghcr.io/zaproxy/zaproxy:stable zap-baseline.py -t http://juice-shop:3000 -r zap-baseline.html
```

The report is written to `reports/zap-baseline.html`. The scan exits with a non-zero code when it finds warnings, which is expected against Juice Shop.

## Project structure

```
.
├── SecurityQa.Tests/
│   ├── Pages/          # Selenium page objects
│   ├── Clients/        # RestSharp API client
│   ├── Web/            # Web UI tests
│   ├── Api/            # API functional tests
│   ├── Security/       # Security tests
│   └── Support/        # Base test class, driver setup, configuration
├── reports/            # Test and scan reports (not committed)
├── .github/workflows/  # CI pipeline
├── .env.example
├── appsettings.json
└── README.md
```

## Troubleshooting

| Problem | Fix |
|---|---|
| `localhost:3000` doesn't load | Check the container is running with `docker ps`. Wait 30 seconds after starting it |
| `network qa-net already exists` | Fine, the network is already there. Carry on |
| `container name "/juice-shop" is already in use` | Start the existing one with `docker start juice-shop` |
| Chrome or driver error on first run | Make sure Chrome is installed and up to date. Selenium fetches the matching driver automatically |
| ZAP can't reach the target | Juice Shop and the scanner must both be on `qa-net`. Use `http://juice-shop:3000` as the target, not `localhost` |

## Roadmap

- [ ] Solution skeleton and tooling
- [ ] Configuration and environment settings
- [ ] Driver lifecycle and base test class
- [ ] Login page object and first web test
- [ ] Explicit waits and data-driven web tests
- [ ] RestSharp API client
- [ ] API functional test suite
- [ ] Authentication and authorization tests (broken auth, BOLA)
- [ ] Input handling tests (injection, mass assignment)
- [ ] Security header and cookie checks
- [ ] ZAP baseline scan
- [ ] Reporting with screenshots on failure
- [ ] GitHub Actions pipeline
- [ ] Architecture notes and findings write-up

## Branching and pull requests

All work happens on `feature/*` branches, merged into `debug` through pull requests. `master` is updated only by release pull requests from `debug`. Both branches are protected against direct pushes.

## Author

<Your Name> · [GitHub](https://github.com/theguptajii07)