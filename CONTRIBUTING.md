# Contributing to Tetris Game Application

Thank you for considering contributing to the Tetris Game Application! This document provides guidelines and steps to help you contribute to this project.

## Table of Contents
- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Pull Requests](#pull-requests)
- [Development Guidelines](#development-guidelines)
  - [Coding Standards](#coding-standards)
  - [Testing](#testing)
  - [Documentation](#documentation)
- [Project Structure](#project-structure)

## Code of Conduct

By participating in this project, you agree to abide by our Code of Conduct. Please be respectful and constructive in your interactions with other contributors.

## Getting Started

1. **Set up your development environment:**
   - Install .NET Core 6.0 SDK or later
   - Install SQL Server/SQLite
   - (Optional) Install Node.js for frontend development

2. **Fork the repository on GitHub**

3. **Clone your fork locally:**
   ```pwsh
   git clone https://github.com/your-username/tetris.git
   cd tetris
   ```

4. **Add the original repository as upstream:**
   ```pwsh
   git remote add upstream https://github.com/original-owner/tetris.git
   ```

5. **Create a feature branch:**
   ```pwsh
   git checkout -b feature/your-feature-name
   ```

## How to Contribute

### Reporting Bugs

If you find a bug, please open an issue using the bug report template. Include:
- A clear and descriptive title
- Steps to reproduce the issue
- Expected behavior
- Screenshots if applicable
- Your environment details (OS, browser, etc.)

### Suggesting Enhancements

If you have an idea for an enhancement:
1. Check if the enhancement has already been suggested
2. Open a new issue using the feature request template
3. Provide a detailed description of the enhancement
4. Explain why this enhancement would be useful

### Pull Requests

1. **Update your fork** with the latest upstream changes:
   ```pwsh
   git fetch upstream
   git merge upstream/main
   ```

2. **Make your changes** adhering to the coding standards

3. **Add tests** for any new functionality

4. **Update documentation** as needed

5. **Commit your changes** following our commit message conventions:
   ```pwsh
   git commit -m "feat: add new feature" # for features
   git commit -m "fix: resolve issue with rotation" # for bug fixes
   git commit -m "docs: update installation instructions" # for documentation
   git commit -m "test: add test for game over condition" # for tests
   ```

6. **Push to your branch:**
   ```pwsh
   git push origin feature/your-feature-name
   ```

7. **Open a pull request** against the main branch of the upstream repository

## Development Guidelines

### Coding Standards

- Follow C# coding conventions and best practices
- Use meaningful variable and method names
- Keep methods focused on a single responsibility
- Write XML documentation for public APIs
- Maintain consistent indentation and formatting

### Testing

- Write unit tests for all new functionality
- Ensure all tests pass before submitting a pull request
- Include integration tests for API endpoints
- Consider adding UI tests for interface changes

### Documentation

- Update README.md and other documentation as needed
- Document public APIs with XML comments
- Include code comments for complex logic
- Consider creating examples for new features

## Project Structure

Please refer to the [README.md](README.md) for details on the project structure.

## Branching Strategy

- `main`: production-ready code
- `develop`: integration branch for features
- `feature/*`: individual feature branches
- `bugfix/*`: individual bug fix branches
- `release/*`: release candidate branches

## Review Process

1. Maintainers will review your pull request
2. Address any feedback or requested changes
3. Once approved, a maintainer will merge your contribution
4. Your contribution will be included in the next release

## Recognition

Contributors will be acknowledged in the project's documentation and release notes.

Thank you for contributing to the Tetris Game Application!
