# AGENTS.md

## Cursor Cloud specific instructions

### Repository overview

This is a general-purpose "TestRepo" used to store isolated technology samples and proof-of-concept projects across separate Git branches. The `main` branch contains only a bare `README.md` — there is no unified application, build system, or service to run.

### Branch structure

Each remote branch holds a self-contained sample (e.g., Angular comparison docs, Kafka producer/consumer samples, .NET MediatR examples). Branches are independent and do not share dependencies or build infrastructure.

### Development environment

- **No dependencies to install:** There is no `package.json`, `requirements.txt`, `Makefile`, `docker-compose.yml`, or any other dependency manifest on `main`.
- **No lint/test/build/run commands:** No tooling is configured on the default branch.
- **No services required:** No databases, message brokers, or other services are needed.

### Working on a specific branch

If working on a branch with actual code (e.g., an Angular or .NET sample), check out that branch and follow any setup instructions present in that branch's files. Each branch is self-contained.
