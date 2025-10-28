# Contributing to ItemsCache

Thank you for your interest in contributing to ItemsCache! This document provides guidelines and information for contributors.

## Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code. Please report unacceptable behavior to the project maintainers.

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Git
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider (recommended)

### Development Setup

1. **Fork and Clone**
   ```bash
   git clone https://github.com/oleksii-mykhniak/ItemsCache.git
   cd ItemsCache
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run Tests**
   ```bash
   dotnet test
   ```

## Development Guidelines

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and small
- Use async/await properly for I/O operations

### Architecture Principles

- Follow SOLID principles
- Use dependency injection
- Implement proper abstractions
- Keep implementations testable
- Maintain backward compatibility

### Testing

- Write unit tests for new functionality
- Maintain test coverage above 80%
- Use descriptive test names
- Test both success and failure scenarios
- Mock external dependencies

## Submitting Changes

### Pull Request Process

1. **Create a Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Your Changes**
   - Write your code following the guidelines
   - Add or update tests
   - Update documentation if needed

3. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "feat: add your feature description"
   ```

4. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

### Commit Message Format

Use conventional commits format:

- `feat:` - New features
- `fix:` - Bug fixes
- `docs:` - Documentation changes
- `style:` - Code style changes
- `refactor:` - Code refactoring
- `test:` - Test additions or changes
- `chore:` - Maintenance tasks

Examples:
- `feat: add background refresh functionality`
- `fix: resolve memory leak in cache service`
- `docs: update API documentation`

### Pull Request Guidelines

- Provide a clear description of changes
- Reference any related issues
- Ensure all tests pass
- Update documentation if needed
- Keep PRs focused and small when possible

## Issue Reporting

### Bug Reports

When reporting bugs, please include:

- Description of the issue
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment details (.NET version, OS, etc.)
- Code samples if applicable

### Feature Requests

For feature requests, please include:

- Description of the feature
- Use case and motivation
- Proposed implementation (if you have ideas)
- Any alternatives considered

## Release Process

Releases are managed through GitHub Actions:

1. **Version Bumping**: Versions are automatically determined based on Git tags
2. **Package Publishing**: Packages are automatically published to NuGet.org
3. **Release Notes**: Changelog is automatically generated from commits

### Creating a Release

1. **Create a Git Tag**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. **GitHub Actions will automatically**:
   - Build and test the code
   - Create NuGet packages
   - Publish to NuGet.org
   - Create a GitHub release
   - Generate release notes

## Development Workflow

### Branch Strategy

- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - Feature development
- `hotfix/*` - Critical bug fixes
- `release/*` - Release preparation

### Continuous Integration

The project uses GitHub Actions for:

- **CI Pipeline**: Build, test, and validate on every PR
- **Code Quality**: SonarCloud analysis
- **Security**: Dependency vulnerability scanning
- **Release**: Automated package publishing

## Documentation

### API Documentation

- Use XML documentation comments for public APIs
- Keep documentation up-to-date with code changes
- Provide examples in documentation

### README Updates

- Update README.md for significant changes
- Include usage examples
- Keep installation instructions current

## Questions?

If you have questions about contributing:

- Open a discussion in the GitHub repository
- Create an issue for specific questions
- Contact the maintainers directly

Thank you for contributing to ItemsCache! 🚀
