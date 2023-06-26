using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Battle;
using Data.Character;
using Data.Inventory;
using PlayFab;
using PlayFab.ServerModels;
using UnityEditor;
using UnityEngine;

namespace Data.Editor
{
    [CustomEditor(typeof(DataBaseContainer))]
    public class DataBaseContainerEditor : UnityEditor.Editor
    {
        private static bool _handledData;
        private static bool _error;
        private static bool _updated;
        private static string[] _allTitleDataKeys;
        private static int _titleDataKeyIndex;

        private static GUIStyle _buttonStyle;
        private static GUIStyle _labelStyle;

        private static DataBaseContainer _container;
        
        public override void OnInspectorGUI()
        {
            if (_handledData)
            {
                if (_error)
                {
                    EditorGUILayout.LabelField(EditorGUIUtility.IconContent("console.erroricon.sml"));
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_WaitSpin00"), GUILayout.Width(22));
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                DrawChooseDataKey();
                DrawExportButtons();
                EditorGUILayout.EndHorizontal();
            }
            
            DrawDefaultInspector();
        }
        
        private void OnEnable()
        {
            _container = (DataBaseContainer)target;
            _buttonStyle = new GUIStyle(EditorStyles.miniButton) { richText = true};
            _labelStyle = new GUIStyle(EditorStyles.label) { richText = true};
            _allTitleDataKeys = Array.Empty<string>();
            
            FetchAllDataAsync(_ =>
            {
                //search difference
                //var localData = ((DataBaseContainer) target).BattlesData;
                _updated = false;
                Debug.LogWarning("Not implemented");
            });
        }
        
        private static void DrawChooseDataKey()
        {
            if (_updated)
            {
                var color = GUI.contentColor;
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_FilterSelectedOnly@2x"), GUILayout.Width(22));
                GUI.contentColor = color;
            }
            else
            {
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_console.warnicon.sml"), GUILayout.Width(22));
            }
            EditorGUILayout.LabelField($"<b>Target data key:</b>", _labelStyle, GUILayout.Width(96));
            _titleDataKeyIndex = EditorGUILayout.Popup(_titleDataKeyIndex, _allTitleDataKeys);
        }
        private static void DrawExportButtons()
        {
            if (GUILayout.Button($"<b>Push</b>", _buttonStyle)) PushDataAsync();
            if (GUILayout.Button($"<b>Pull</b> ", _buttonStyle)) FetchAllDataAsync(PullFromKey);
            if (GUILayout.Button($"Export <b>.json</b>", _buttonStyle)) ExportJson();
        }

        private static void ExportJson()
        {
            switch (_allTitleDataKeys[_titleDataKeyIndex])
            {
                case Constants.PLAYFAB_GAME_BATTLES:
                    Json.Generator.Export(Application.dataPath, _container.BattlesData);
                    break;
                
                case Constants.PLAYFAB_GAME_CHARACTERS:
                    Json.Generator.Export(Application.dataPath, _container.BattlesData);
                    break;
                
                case Constants.PLAYFAB_GAME_ITEMS:
                    Json.Generator.Export(Application.dataPath, _container.ItemsData);
                    break;
                
                case Constants.PLAYFAB_GAME_SHOP_ITEMS:
                    Json.Generator.Export(Application.dataPath, _container.ShopData);
                    break;
            }
            AssetDatabase.Refresh();
        }
        private static void PullFromKey(Dictionary<string, string> allData)
        {
            var key = _allTitleDataKeys[_titleDataKeyIndex];
            switch (key)
            {
                case Constants.PLAYFAB_GAME_BATTLES:
                    _container.BattlesData = JsonUtility.FromJson<BattlesData>(allData[key]);
                    break;
                
                case Constants.PLAYFAB_GAME_CHARACTERS:
                    _container.CharactersData = JsonUtility.FromJson<CharactersData>(allData[key]);
                    break;
                
                case Constants.PLAYFAB_GAME_ITEMS:
                    _container.ItemsData = JsonUtility.FromJson<ItemsData>(allData[key]);
                    break;
                
                case Constants.PLAYFAB_GAME_SHOP_ITEMS:
                    _container.ShopData = JsonUtility.FromJson<ShopData>(allData[key]);
                    break;
            }
            _updated = true;
        }
        private static async void FetchAllDataAsync(Action<Dictionary<string, string>> callback = default)
        {
            if (_handledData) return;
            var allData = await GetAllTitleData();
            var remoteKeys = allData.Keys.ToArray();
            if(_allTitleDataKeys.Length != remoteKeys.Length) _titleDataKeyIndex = 0;
            _allTitleDataKeys = remoteKeys;
            callback?.Invoke(allData);
        }
        private static async Task<Dictionary<string, string>> GetAllTitleData()
        {
            if (_handledData) return default;
            _handledData = true;
            _error = false;
            Dictionary<string, string> remoteData = default;
            PlayFabServerAPI.GetTitleData(
                new GetTitleDataRequest(), 
                result => { _handledData = false; remoteData = result.Data; },
                error => { Debug.Log(error.GenerateErrorReport()); _handledData = false; _error = true; });
            while (_handledData) await Task.Yield();
            return remoteData;
        }
        private static async void PushDataAsync()
        {
            if (_handledData) return;
            var data = _allTitleDataKeys[_titleDataKeyIndex] switch
            {
                Constants.PLAYFAB_GAME_BATTLES    => JsonUtility.ToJson(_container.BattlesData, true),
                Constants.PLAYFAB_GAME_CHARACTERS => JsonUtility.ToJson(_container.CharactersData, true),
                Constants.PLAYFAB_GAME_ITEMS      => JsonUtility.ToJson(_container.ItemsData, true),
                Constants.PLAYFAB_GAME_SHOP_ITEMS => JsonUtility.ToJson(_container.ShopData, true),
                _ => string.Empty
            };
            await SetTitleData(_allTitleDataKeys[_titleDataKeyIndex], data);
        }
        private static async Task SetTitleData(string key, string data) 
        {
            if (_handledData) return;
            _handledData = true;
            _error = false;
            _updated = false;
            PlayFabServerAPI.SetTitleData(
                new SetTitleDataRequest { Key = key, Value = data },
                _ => { _handledData = false; _updated = true; },
                error => { Debug.Log(error.GenerateErrorReport()); _handledData = false; _error = true; });
            while (_handledData == false) await Task.Yield();
        }
    }
}