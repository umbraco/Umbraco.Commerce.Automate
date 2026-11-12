---
name: post-release-cleanup
description: Merges a released release/* branch back into its line (main, support/17.x, or v19/dev), tags the merge commit, creates the GitHub Release, bumps version.json for next-cycle nightlies, and deletes the release branch. Use after the release build has published the package to MyGet and NuGet.org.
allowed-tools: Bash, Read, Edit, AskUserQuestion
---

# Post-Release Cleanup (satellite)

Run this **after** the release branch's CI build has published the package: its Prerelease stage
(MyGet) and its Release stage (NuGet.org, after a manual approval in the run) both succeeded. This is
the point where a version is actually shipped, so tagging and creating the GitHub Release happen here,
not on branch push.

## Workflow

1. **Confirm the release actually shipped.** Ask the user (AskUserQuestion) to confirm the release
   branch's build published to MyGet and NuGet.org (Prerelease and Release stages green). If not
   confirmed, stop — do not tag or merge an unpublished release.

2. **Identify the release branch and its target:**
   - `release/*` branches cut from `main` merge back into `main` (v18)
   - `release/*` branches cut from `support/17.x` merge back into `support/17.x` (v17)
   - `release/*` branches cut from `v19/dev` merge back into `v19/dev` (v19 pre-release). Never delete
     `v19/dev`, and don't merge a v19 release branch into `main` while `main` is the v18 line.

   If ambiguous, ask the user.

3. **Fetch latest state:**
   ```bash
   git fetch origin --tags
   ```

4. **Merge the release branch into its target:**
   ```bash
   git checkout main                  # or support/17.x, v19/dev
   git pull origin main                # or support/17.x, v19/dev
   git merge --no-ff origin/release/<name> -m "Merge release/<name> into <target>"   # <target> = the branch checked out above
   ```

5. **Read the shipped version** from `version.json` on the merge commit (this is the released version set
   by `/release-management`, e.g. `18.0.0`, or a pre-release such as `19.0.0-beta.2` on `v19/dev`).

6. **Tag the merge commit and push the tag:**
   ```bash
   git tag release-<version>
   git push origin release-<version>
   ```
   Tag naming is `release-<version>` (e.g. `release-18.0.0`, `release-17.0.0`) — matches the existing
   tag convention in this repo.

7. **Create the GitHub Release:**
   ```bash
   gh release create release-<version> --target main --generate-notes
   ```
   (`--target support/17.x` for a v17 release, `--target v19/dev` for a v19 pre-release.) Use `--generate-notes` — GitHub's auto-generated
   summary of merged PRs since the previous release. Do not hand-write release notes.

8. **Push the merge commit:**
   ```bash
   git push origin main               # or support/17.x, v19/dev
   ```

9. **Bump `version.json` on the target branch** for the next cycle:
   - **Stable release:** patch-bump (e.g. `18.0.0` → `18.0.1`) so nightly `--preview.*` builds sort
     above the just-shipped version.
   - **Pre-release** (a suffix such as `-beta`): move to the next pre-release name, e.g. `19.0.0-beta`
     → `19.0.0-beta.2`, or → `19.0.0-rc`, as the release manager chooses. Never patch-bump into a
     stable version. The next `release/*` branch must carry a version that isn't published yet.
   - **Pre-release nightlies sort below what shipped.** Non-public builds format as
     `{version}--{prerelease}preview.{height}+{commit}`, so a `19.0.0-beta.2` nightly is
     `19.0.0--beta.2.preview.N`, which sorts below a shipped `19.0.0-beta`. This is expected for
     pre-releases; don't rely on the suffix bump to keep nightlies above them.
   - A pre-release is tagged with its full version (e.g. `release-19.0.0-beta`).

   Commit as:
   ```bash
   git commit -am "chore(release): Bump <branch> to <new-version> after <release-branch-name> release"
   git push origin main                # or support/17.x, v19/dev
   ```

10. **Delete the release branch, local and remote:**
    ```bash
    git branch -d release/<name>
    git push origin --delete release/<name>
    ```

11. **Report summary:**
    ```
    ✓ Merged release/2026.07.3 into main
    ✓ Tagged release-18.0.0 and pushed
    ✓ Created GitHub Release release-18.0.0 (auto-generated notes)
    ✓ Bumped main to 18.0.1 for next-cycle nightlies
    ✓ Deleted release/2026.07.3 (local + remote)
    ```

## Notes

- Always run from repository root.
- This is a light version of the core Umbraco.Automate `post-release-cleanup` skill — no changelog
  files, no release manifest, no multi-product tag scanning (single product, one `version.json`).
- Do not tag or create a release for a branch whose package was not actually published.
- If no `release/*` branch matches, or the merge conflicts, stop and report — do not force-resolve
  conflicts silently.
