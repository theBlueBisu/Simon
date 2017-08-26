﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SimonComponent.ActionScripts;
using UnityEngine;
using UnityEditor;

namespace SimonComponent
{
    [CustomEditor(typeof(SimonDisplayer))]
    // [CanEditMultipleObjects]
    public class SimonDisplayerEditor : Editor
    {
        private int indexPopupAddDAS = 0;
        
        public override void OnInspectorGUI()
        {
            SimonDisplayer script = (SimonDisplayer) target;

            // Simon Player
            script.SimonPlayer = (SimonPlayer) EditorGUILayout.ObjectField(new GUIContent("Simon Player"), (!script.ListenFromManager ? script.SimonPlayer : null), typeof(SimonPlayer), true);

            // Due of the runing in Editor, the Instance is not necessarily set,
            // so force it by the call of the property if there is a component SimonManager in the scene
            if (FindObjectOfType<SimonManager>() != null)
            {
                if (SimonManager.Instance == null) { return; }
            }
            
            // If there is no Simon player linked
            if (script.SimonPlayer == null)
            {
                EditorGUILayout.HelpBox("Require a SimonPlayer linked or a SimonManager in the scene", MessageType.Warning, true);
            }
            // Else display more options
            else
            {
                if (script.ListenFromManager)
                    EditorGUILayout.HelpBox("Listening from the SimonManager in the scene", MessageType.None, true);
                
                // Symbol listened
                EditorGUILayout.BeginHorizontal();
                // If there is no symbol in the player, display a message
                if (script.SimonPlayer.Symbols.Length == 0)
                {
                    EditorGUILayout.HelpBox("No symbols set in the SimonPlayer listened\n\nYou must add at least one symbol to configure the displayer", MessageType.Warning, true);
                }
                // Else, draw a popup of symbols
                else
                {
                    EditorGUILayout.PrefixLabel("Symbol");
                    int symbolIndex = script.SimonPlayer.GetSymbolIndex(script.ListenedSymbol);
                    // Symbol (in string) not refound
                    if (symbolIndex < 0)
                    {
                        // Try to use the registered index
                        if (script.ListenedSymbolIndex > 0 && script.ListenedSymbolIndex < script.SimonPlayer.Symbols.Length)
                            symbolIndex = script.ListenedSymbolIndex;
                        // Else, by default the first symbol
                        else
                            symbolIndex = 0;
                    }
                    script.ListenedSymbol = script.SimonPlayer.Symbols[EditorGUILayout.Popup(symbolIndex, script.SimonPlayer.Symbols)];
                }
                EditorGUILayout.EndHorizontal();
                
                // Actions
                EditorGUILayout.Separator();
                
                GUILayout.Label("Actions scripts");

                var types = AbstractDAS.GetDisplayerActionScripts();
                var typeNames = new List<string>();
                foreach (Type type in types)
                    typeNames.Add(type.Name.Replace("DAS", ""));

                EditorGUILayout.BeginHorizontal();
                indexPopupAddDAS = EditorGUILayout.Popup(indexPopupAddDAS, typeNames.ToArray());
                if (GUILayout.Button("Add", GUILayout.MaxWidth(40), GUILayout.Height(14)))
                    script.ActionScripts.Add((AbstractDAS) ScriptableObject.CreateInstance(types[indexPopupAddDAS].Name));
                EditorGUILayout.EndHorizontal();
                
                for (int i = 0, count = script.ActionScripts.Count; i < count; i++)
                {
                    var actionScript = script.ActionScripts[i];
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("x", GUILayout.MaxWidth(18), GUILayout.Height(14)))
                    {
                        script.ActionScripts.RemoveAt(i);
                        i--;
                        count--;
                        continue;
                    }
                    GUILayout.Label(actionScript.GetActionScriptName());
                    EditorGUILayout.EndHorizontal();
                    CreateEditor(actionScript).OnInspectorGUI();
                    
                    EditorGUILayout.Separator();
                }
                
                EditorGUILayout.Separator();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}
