# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial release of ItemsCache library
- Core caching functionality with `IItemsCacheService<T>`
- Background refresh capabilities with polling implementation
- Retry policy integration with Polly
- Modular package architecture with separate abstraction and implementation packages
- Comprehensive documentation and examples
- GitHub Actions CI/CD pipeline
- Automated package publishing to NuGet.org
- Code quality analysis with SonarCloud
- Dependabot for dependency updates

### Packages
- **ItemsCache.Core.Abstraction** - Core interfaces and abstractions
- **ItemsCache.Core** - Main caching implementation
- **ItemsCache.Refresh.Abstraction** - Refresh functionality abstractions
- **ItemsCache.Refresh.Core** - Core refresh implementation
- **ItemsCache.Refresh.Polling.Abstraction** - Polling-specific abstractions
- **ItemsCache.Refresh.Polling** - Polling-based refresh implementation
- **ItemsCache.RetryPolicy.Abstraction** - Retry policy abstractions
- **ItemsCache.RetryPolicy.Polly** - Polly-based retry implementation
- **ItemsCache.All** - Complete package with all features

## [1.0.0] - 2024-01-XX

### Added
- Initial release
- Core caching functionality
- Background refresh with polling
- Retry policies with Polly integration
- Comprehensive documentation
- Sample API application
- Unit tests and integration tests
- CI/CD pipeline with GitHub Actions
- Automated package publishing
- Code quality analysis
- Dependency management with Dependabot
