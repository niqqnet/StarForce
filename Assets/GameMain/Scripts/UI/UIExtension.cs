﻿using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class UIExtension
    {
        public static IEnumerator Fade(this CanvasGroup group, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = group.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                group.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            group.alpha = alpha;
        }

        public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            return uiComponent.HasUIForm((int)uiFormId, uiGroup);
        }

        public static UGuiForm GetUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            UnityGameFramework.Runtime.UIForm uiForm = uiComponent.GetUIForm((int)uiFormId, uiGroup);
            if (uiForm == null)
            {
                return null;
            }

            return (UGuiForm)uiForm.Logic;
        }

        public static UGuiForm[] GetUIForms(this UIComponent uiComponent, UIFormId uiFormId, string uiGroup = "Default")
        {
            UnityGameFramework.Runtime.UIForm[] uiForms = uiComponent.GetUIForms((int)uiFormId, uiGroup);
            List<UGuiForm> uiFormList = new List<UGuiForm>();
            for (int i = 0; i < uiForms.Length; i++)
            {
                uiFormList.Add((UGuiForm)uiForms[i].Logic);
            }

            return uiFormList.ToArray();
        }

        public static void CloseUIForm(this UIComponent uiComponent, UGuiForm uiForm)
        {
            uiComponent.CloseUIForm(uiForm.UIForm);
        }

        public static void OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
        {
            uiComponent.OpenUIForm((int)uiFormId, userData);
        }

        public static void OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return;
            }

            uiComponent.OpenUIForm(uiFormId, AssetUtility.GetUIFormAsset(drUIForm.AssetName), drUIForm.UIGroupName, drUIForm.PauseCoveredUIForm, userData);
        }

        public static void OpenDialog(this UIComponent uiComponent, UIDialogParams dialogParams)
        {
            if (((ProcedureBase)GameEntry.Procedure.CurrentProcedure).UseNativeDialog)
            {
                OpenNativeDialog(dialogParams);
            }
            else
            {
                uiComponent.OpenUIForm(UIFormId.Dialog, dialogParams);
            }
        }

        private static void OpenNativeDialog(UIDialogParams dialogParams)
        {
            throw new System.NotImplementedException("OpenNativeDialog");
        }

        private interface INativeCaller
        {
            void OpenDialog(int mode, string title, string message,
                string confirmText, int confirmCallbackId,
                string cancelText, int cancelCallbackId,
                string otherText, int otherCallbackId,
                string userData);
        }
    }
}
