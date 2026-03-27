---
name: Pre-PR Review
description: "Use when: before opening a pull request, pre-PR check, review current branch changes, catch blockers early."
argument-hint: "Base branch or diff target (default: main)"
agent: "PR Reviewer"
---

Run a strict pre-PR review for this repository.

If an argument is provided, use it as the diff target/base branch.
If no argument is provided, compare the current work against `main`.

Review the current branch changes and return:

1. Blockers (must fix before merge)
2. Warnings (should fix)
3. Passed checks
4. A short merge-readiness summary

Prefer reading the branch diff directly. If no diff is available, ask me for the PR number, branch, or pasted diff.
