using System.Reflection;
using SPT.Reflection.Patching;
using UnityEngine;
using VisorEffectManager;

namespace VisorEffectManager.Patches
{
    internal class FaceShieldPatch : ModulePatch
    {

        // SPT 4.1 deobfuscated the client: VisorEffect.method_2 is now SetDefault().
        // It is the method that assigns all five visor textures, so a postfix here is
        // still exactly the right place to null the ones the user turned off.
        protected override MethodBase GetTargetMethod()
        {
            return typeof(VisorEffect).GetMethod(nameof(VisorEffect.SetDefault), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }

        [PatchPostfix]
        static void Postfix(VisorEffect __instance)
        {
            try
            {
                // Verifica se a instância é válida
                if (__instance == null)
                {
                    VisorEffectManager.LogSource?.LogWarning("FaceShieldPatch: VisorEffect instance is null");
                    return;
                }

                // Obtém o material  (4.0: method_4)
                Material material = __instance.GetMaterial();

                // Verifica se o material é válido antes de aplicar configurações
                if (material == null)
                {
                    VisorEffectManager.LogSource?.LogWarning("FaceShieldPatch: Material is null, skipping configuration application");
                    return;
                }

                // Aplica configurações apenas se as entradas de configuração estiverem inicializadas
                if (VisorEffectManager.RemoveGlassDamage == null ||
                    VisorEffectManager.RemoveScratches == null ||
                    VisorEffectManager.RemoveBlur == null ||
                    VisorEffectManager.RemoveDistortion == null)
                {
                    VisorEffectManager.LogSource?.LogWarning("FaceShieldPatch: Configuration entries not initialized yet");
                    return;
                }

                // Remove texturas baseado nas configurações
                if (VisorEffectManager.RemoveGlassDamage.Value)
                {
                    material.SetTexture("_GlassDamageTex", null);
                }

                if (VisorEffectManager.RemoveScratches.Value)
                {
                    material.SetTexture("_ScratchesTex", null);
                }

                if (VisorEffectManager.RemoveBlur.Value)
                {
                    material.SetTexture("_BlurMask", null);
                }

                if (VisorEffectManager.RemoveDistortion.Value)
                {
                    material.SetTexture("_DistortMask", null);
                }

                VisorEffectManager.LogSource?.LogDebug($"FaceShieldPatch: Applied settings to visor effect (Material: {material.name})");
            }
            catch (System.Exception ex)
            {
                VisorEffectManager.LogSource?.LogError($"FaceShieldPatch: Error applying visor settings - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
