<p align="center">
  <a href="README.md"><img alt="한국어" src="https://img.shields.io/badge/%ED%95%9C%EA%B5%AD%EC%96%B4-%ED%95%9C%EA%B5%AD%EC%96%B4%EB%A1%9C%20%EB%B3%B4%EA%B8%B0-6e7681?style=for-the-badge"></a>
  <a href="README.en.md"><img alt="English (this page)" src="https://img.shields.io/badge/English-This%20page-1f6feb?style=for-the-badge"></a>
</p>

### ⚠️ IMPORTANT NOTICE / DISCLAIMER

- **Original Author:** silviohmartins
- **Original Repository:** VisorEffectManager
- **Original Link:** https://github.com/silviohmartins/VisorEffectManager
- **License:** MIT
- **This Port By:** R_F (danyhappy564-cmyk) — unofficial, AI-assisted port. Not affiliated with or endorsed by the original author.

1. **Reflection & Take-Downs:** I deeply reflect on the ECOT incident. As an AI-assisted "vibe coder," I will immediately delete files if the original authors ask.
2. **No Re-Distribution:** These ported builds are unverified, temporary fixes. Please do NOT re-upload or share them anywhere else.
3. **Do Not Pester Original Authors:** Never report bugs or pester original modders regarding issues from my unofficial ports.
4. **Full Credit & Respect:** I will always credit original creators on GitHub and prioritize their decisions above all else.
5. **Support Original Creators:** Instead of using my ports, please visit the original authors' Forge pages to leave kind words or tips.

---

## Changelog

The full changelog (1 entry) is in the [`changelog/`](changelog/) folder, 10 per file (higher number = newer, latest: [`CHANGELOG-001.md`](changelog/CHANGELOG-001.md)). Per-release summaries: [Releases](../../releases).

# VisorEffectManager 4.1

A BepInEx plugin that disables visual effects shown while wearing a visor (face shield). Ported to **SPT 4.1.5**.

> Original: **silviohmartins** · MIT license
> This repository is an SPT 4.1 port of the original.

## What does it do?

Lowering a face shield/visor overlays glass damage, scratches, blur, and distortion on the screen. This mod lets you disable each effect separately.

| Effect | Shader property | Default |
| --- | --- | --- |
| Glass damage | `_GlassDamageTex` | Removed |
| Scratches | `_ScratchesTex` | Removed |
| Blur | `_BlurMask` | Removed |
| Distortion | `_DistortMask` | Removed |

## Installation

Place `VisorEffectManager.dll` in `BepInEx\plugins\`. There is no server mod.

## Build

```
dotnet build VisorEffectManager.csproj -c Release
```

The default SPT path is `E:\SPT 4.1`. If yours is different:

```
dotnet build VisorEffectManager.csproj -c Release -p:SptRoot="D:\내SPT경로"
```

The build automatically copies the file into `BepInEx\plugins\`. (The old `TarkovDir` property still works.)

## F12 settings (BepInEx Configuration Manager)

**Face Shield Settings** — four on/off switches, one per effect
**Keyboard Shortcuts** — toggle each effect with `RightCtrl + 1~4` by default

> Keyboard shortcuts **do not apply changes immediately.** Changes take effect when you raise and lower the visor, change helmets,
> or start a new raid. The original author added live updates (`OnSettingChanged` /
> `UpdateAllVisorEffects`), then removed them because they caused conflicts. This port follows that decision.

## Changes in the 4.1 port

SPT 4.1 deobfuscates the EFT client. This mod used **2** obfuscated names,
which were checked against the actual 4.1.5 `Assembly-CSharp.dll` and updated as follows.

| 4.0 (obfuscated) | 4.1 (actual name) | Purpose |
| --- | --- | --- |
| `VisorEffect.method_2` | `VisorEffect.SetDefault()` | Sets all 5 visor textures — the patch target |
| `VisorEffect.method_4()` | `VisorEffect.GetMaterial()` | Returns the visor material |

These were identified by matching 4.0 ↔ 4.1 methods one-to-one using IL size and declaration order (`method_2` = il 91 = `SetDefault`,
`method_4` = il 42 = `GetMaterial`). The `VisorEffect` type name and all
4 shader property strings stayed the same.

Other changes:

- Replaced the hardcoded build path (`D:\GAMES\SPT-4.0.2_MODDING\`) with `SptRoot`
- Added an automatic deployment target for `BepInEx\plugins\` (the original only had `RunPostBuildEvent`, without an actual copy step)
- Updated the README — the removed Mask option and the claim of live updates did not match the code

## Verification

Successful compilation and Harmony finding its targets at runtime are different things, so these were checked against the actual 4.1.5 assemblies:

- `SetDefault` / `GetMaterial` are both **public instance methods declared directly on `VisorEffect`** →
  the mod's `BindingFlags.Instance | Public | DeclaredOnly` finds them
- **No methods in `VisorEffect` share a name** → no risk of `AmbiguousMatchException`
- The `SetDefault` body sets 5 textures: `_Mask` `_ScratchesTex` `_BlurMask` `_DistortMask`
  `_GlassDamageTex` → all 4 textures removed by the mod are included, confirming the patch location

## Status

- Build: **successful**
- In-game testing: **not yet performed**
