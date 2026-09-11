### ⚠️ IMPORTANT NOTICE / DISCLAIMER

**Original Author:** silviohmartins
**Original Repository:** VisorEffectManager
**Original Link:** https://github.com/silviohmartins/VisorEffectManager
**License:** MIT
**This Port By:** R_F (danyhappy564-cmyk) — unofficial, AI-assisted port. Not affiliated with or endorsed by the original author.

1. **Reflection & Take-Downs:** I deeply reflect on the ECOT incident. As an AI-assisted "vibe coder," I will immediately delete files if the original authors ask.
2. **No Re-Distribution:** These ported builds are unverified, temporary fixes. Please do NOT re-upload or share them anywhere else.
3. **Do Not Pester Original Authors:** Never report bugs or pester original modders regarding issues from my unofficial ports.
4. **Full Credit & Respect:** I will always credit original creators on GitHub and prioritize their decisions above all else.
5. **Support Original Creators:** Instead of using my ports, please visit the original authors' Forge pages to leave kind words or tips.

---

# VisorEffectManager 4.1

바이저(안면보호구) 착용 시 화면에 끼는 시각 효과를 끄는 BepInEx 플러그인. **SPT 4.1.5** 용으로 포팅했습니다.

> 원작: **silviohmartins** · MIT 라이선스
> 이 저장소는 원작의 SPT 4.1 포팅입니다.

## 뭐 하는 모드냐

페이스실드/바이저를 내리면 화면에 유리 손상·스크래치·블러·왜곡이 겹쳐 보이는데, 그걸 항목별로 끕니다.

| 효과 | 셰이더 프로퍼티 | 기본값 |
| --- | --- | --- |
| 유리 손상 | `_GlassDamageTex` | 제거 |
| 스크래치 | `_ScratchesTex` | 제거 |
| 블러 | `_BlurMask` | 제거 |
| 왜곡 | `_DistortMask` | 제거 |

## 설치

`VisorEffectManager.dll` 을 `BepInEx\plugins\` 에 넣으면 끝입니다. 서버 모드는 없습니다.

## 빌드

```
dotnet build VisorEffectManager.csproj -c Release
```

SPT 경로 기본값은 `E:\SPT 4.1` 입니다. 다르면:

```
dotnet build VisorEffectManager.csproj -c Release -p:SptRoot="D:\내SPT경로"
```

빌드하면 `BepInEx\plugins\` 로 자동 복사됩니다. (예전 `TarkovDir` 속성도 그대로 먹습니다.)

## F12 설정

**Face Shield Settings** — 효과별 on/off 4개
**Keyboard Shortcuts** — 기본 `RightCtrl + 1~4` 로 각각 토글

> 단축키를 눌러도 **즉시 반영되지는 않습니다.** 바이저를 올렸다 내리거나, 헬멧을 바꾸거나,
> 새 레이드를 시작하면 적용됩니다. 원작자가 실시간 갱신 기능(`OnSettingChanged` /
> `UpdateAllVisorEffects`)을 넣었다가 충돌이 나서 직접 걷어냈고, 이 포팅도 그 결정을 따랐습니다.

## 4.1 포팅에서 바뀐 것

SPT 4.1은 EFT 클라이언트의 난독화를 해제합니다. 이 모드가 쓰는 난독화 이름은 **2개**였고,
실제 4.1.5 `Assembly-CSharp.dll` 로 확인해서 아래처럼 맞췄습니다.

| 4.0 (난독화) | 4.1 (실제 이름) | 하는 일 |
| --- | --- | --- |
| `VisorEffect.method_2` | `VisorEffect.SetDefault()` | 바이저 텍스처 5개를 전부 세팅 — 패치 대상 |
| `VisorEffect.method_4()` | `VisorEffect.GetMaterial()` | 바이저 머티리얼 반환 |

IL 크기와 선언 순서로 4.0 ↔ 4.1 메서드를 1:1 대조해서 찾았습니다 (`method_2` = il 91 = `SetDefault`,
`method_4` = il 42 = `GetMaterial`). `VisorEffect` 타입 이름 자체와 셰이더 프로퍼티 문자열
4개는 그대로였습니다.

그 외:

- 빌드 경로를 하드코딩(`D:\GAMES\SPT-4.0.2_MODDING\`)에서 `SptRoot` 로 교체
- `BepInEx\plugins\` 자동 배포 타깃 추가 (원본은 `RunPostBuildEvent` 만 있고 실제 복사 단계가 없었음)
- README 갱신 — 지워진 Mask 항목과 "실시간 반영" 문구가 코드와 안 맞았습니다

## 검증

컴파일이 되는 것과 런타임에 Harmony가 대상을 찾는 것은 다른 문제라, 실제 4.1.5 어셈블리로 확인했습니다:

- `SetDefault` / `GetMaterial` 둘 다 **public instance, `VisorEffect` 에 직접 선언** →
  모드가 넘기는 `BindingFlags.Instance | Public | DeclaredOnly` 로 잡힙니다
- `VisorEffect` 안에 **이름이 겹치는 메서드가 하나도 없음** → `AmbiguousMatchException` 위험 없음
- `SetDefault` 본문이 세팅하는 텍스처가 `_Mask` `_ScratchesTex` `_BlurMask` `_DistortMask`
  `_GlassDamageTex` 5개 → 모드가 지우는 4개가 전부 여기 포함됨. 패치 지점이 맞습니다

## 상태

- 빌드: **성공**
- 인게임 테스트: **아직 안 함**
