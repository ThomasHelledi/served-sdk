# Contributing to Served.SDK

Thank you for your interest in contributing to Served.SDK!

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/YOUR_USERNAME/served-sdk.git`
3. Create a branch: `git checkout -b feature/your-feature`
4. Make your changes
5. Test your changes: `dotnet test`
6. Commit: `git commit -m "Add your feature"`
7. Push: `git push origin feature/your-feature`
8. Create a Pull Request

## Development Setup

```bash
# Clone
git clone https://github.com/ThomasHelledi/served-sdk.git
cd served-sdk

# Build
dotnet build

# Test
dotnet test

# Pack locally
dotnet pack -c Release
```

## Code Style

- Follow C# naming conventions
- Use async/await for all API calls
- Add XML documentation for public APIs
- Write unit tests for new functionality

## Pull Request Guidelines

- Keep PRs focused on a single feature or fix
- Update documentation if needed
- Add tests for new functionality
- Ensure all tests pass
- Update CHANGELOG.md

## Questions?

- Open an issue on GitHub
- Join our Discord: https://discord.gg/unifiedhq

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
