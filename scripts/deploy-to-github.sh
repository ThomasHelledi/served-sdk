#!/bin/bash
# Deploy SDK docs to GitHub
# Usage: ./scripts/deploy-to-github.sh

set -e

REPO_NAME="served-sdk"
ORG="UnifiedHQ"
DOCS_DIR="$(cd "$(dirname "$0")/.." && pwd)"

echo "=== Deploying SDK Docs to GitHub ==="
echo "Directory: $DOCS_DIR"
echo "Target: github.com/$ORG/$REPO_NAME"
echo ""

# Check if gh CLI is available
if ! command -v gh &> /dev/null; then
    echo "Error: GitHub CLI (gh) is required"
    echo "Install: brew install gh"
    exit 1
fi

# Check if authenticated
if ! gh auth status &> /dev/null; then
    echo "Error: Not authenticated with GitHub"
    echo "Run: gh auth login"
    exit 1
fi

# Create repo if it doesn't exist
if ! gh repo view "$ORG/$REPO_NAME" &> /dev/null 2>&1; then
    echo "Creating repository $ORG/$REPO_NAME..."
    gh repo create "$ORG/$REPO_NAME" \
        --public \
        --description "Official .NET SDK for Served and UnifiedHQ platforms (Documentation)" \
        --homepage "https://docs.served.dk/sdk"
    echo "Repository created!"
else
    echo "Repository already exists"
fi

# Initialize git if needed
cd "$DOCS_DIR"
if [ ! -d ".git" ]; then
    git init
    git add -A
    git commit -m "Initial SDK documentation"
fi

# Add/update remote
if git remote get-url github &> /dev/null 2>&1; then
    git remote set-url github "git@github.com:$ORG/$REPO_NAME.git"
else
    git remote add github "git@github.com:$ORG/$REPO_NAME.git"
fi

# Push
echo "Pushing to GitHub..."
git push -u github main --force

echo ""
echo "=== Deployment Complete ==="
echo "URL: https://github.com/$ORG/$REPO_NAME"
echo "Pages: https://$ORG.github.io/$REPO_NAME (enable in repo settings)"
