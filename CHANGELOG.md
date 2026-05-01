# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Generated from [Conventional Commits](https://www.conventionalcommits.org/) via [release-please](https://github.com/googleapis/release-please).

## [2.0.0](https://github.com/anthonynoelw/dotnet-project-template/compare/v1.0.0...v2.0.0) (2026-05-01)


### ⚠ BREAKING CHANGES

* **health:** Removes GET /api/v1/status endpoint (StatusController). Use GET /health or GET /health/ready.

### Features

* add Docker support with Dockerfile and docker-compose ([2845b68](https://github.com/anthonynoelw/dotnet-project-template/commit/2845b68dfcaba12255559558d03125a35e733880))
* add global exception handler and domain exception types ([af7eaef](https://github.com/anthonynoelw/dotnet-project-template/commit/af7eaef706a2a8ddd2d78e8f7ab72e812d1d6a36))
* add solution, source projects, and tests with central NuGet management ([c041b79](https://github.com/anthonynoelw/dotnet-project-template/commit/c041b79e12ccfb6a920c348041c0b46a6fcb9440))
* added codeowners template file ([1d1abff](https://github.com/anthonynoelw/dotnet-project-template/commit/1d1abff1c4f077a6e2976c862b612780883b1677))
* **ai-workflow:** added claude.md file for token efficiency ([1005c1f](https://github.com/anthonynoelw/dotnet-project-template/commit/1005c1fa2f403f0137c69d414a768c46dc04da3f))
* application and integration test projects ([0ddb76a](https://github.com/anthonynoelw/dotnet-project-template/commit/0ddb76aada2d982be81a57aed109a4be71404aee))
* **ci/cd:** added github actions ([dbec211](https://github.com/anthonynoelw/dotnet-project-template/commit/dbec211e4c9c523fa30eb602c777e459fa53449a))
* **health:** add /health and /health/ready infrastructure endpoints ([6bcc6be](https://github.com/anthonynoelw/dotnet-project-template/commit/6bcc6beec993711200584f3651c17a55fde5f843))
* **logging:** integrate Serilog structured logging in Api and Agent ([bfb70d8](https://github.com/anthonynoelw/dotnet-project-template/commit/bfb70d86f32bb6768f555133207890167509e902))
* setup automated release workflow with release-please ([3fbbf71](https://github.com/anthonynoelw/dotnet-project-template/commit/3fbbf719d5439a8befe6c150173216c0be1aa1f7))
* **versioning:** added api versioning with tests ([85156ef](https://github.com/anthonynoelw/dotnet-project-template/commit/85156ef24897542e55af1337645d8e53e569fb50))


### Bug Fixes

* fixed documentation skill file ([924ff37](https://github.com/anthonynoelw/dotnet-project-template/commit/924ff377e4ad8bd9d68a9f2550f548d0490ab7c8))
* **release:** add missing manifest file and update to Node.js 24 ([9e1087e](https://github.com/anthonynoelw/dotnet-project-template/commit/9e1087e4774413b009ae87fa897f62cfc6495124))
* **release:** add missing manifest file and update to Node.js 24 ([2974431](https://github.com/anthonynoelw/dotnet-project-template/commit/2974431d3b71fbd902009a1f75bb583658d4e19a))
* **release:** release.yml file ([7574836](https://github.com/anthonynoelw/dotnet-project-template/commit/7574836e2c5a6d5121d1e8049e622ca99be95ecf))
* tighten editorconfig rules and add StyleCop diagnostic suppressions ([634fc1f](https://github.com/anthonynoelw/dotnet-project-template/commit/634fc1f7f58f031b5893522a74911dd262462505))

## [Unreleased]

### Added
- API versioning with tests
- Structured logging with Serilog in Api and Agent
- GitHub Actions CI/CD pipeline
- Integration and Application test projects
- Global exception handling with RFC 9457 Problem Details

### Changed
- Re-evaluated project features and template structure
- Relocated exception handler tests to Application project
- Adopted FluentAssertions for test assertions

## Development Notes

Releases follow [Semantic Versioning](https://semver.org/):
- **Major** (X.0.0): Breaking changes
- **Minor** (0.X.0): New features (backwards compatible)
- **Patch** (0.0.X): Bug fixes

Version bumping and changelog updates are automated via conventional commits:
- `feat:` → Minor bump
- `fix:` → Patch bump
- `BREAKING CHANGE:` → Major bump
