using System.Collections.Generic;
using System.IO;
using EDITOR.SYSTEMS;
using LitJson;
using UnityEngine;

namespace EDITOR.EXPORT
{
    public class CreatureExporter
    {
        //Here we export a creatures.dbase file where all designed creatures are stored
        private const string databaseExtention = ".dbase";

        public void SaveCreatureDatabase(string path)
        {
            //Convert this to compress the creatures & export them!
            //List<ExportCreatures> exportGroups = new List<ExportCreatures>();
            //foreach (var group in ItemPoolEditor.itemGroups)
            //{
            //    exportGroups.Add(new ExportCreatures(group));
            //}
        }
    }
    
    public struct ExportCreatures
    {
        //public ExportItemGroup[] exportGroups;
        //
        //public ExportCreatures()
        //{
        //    this.exportGroups = exportGroups.ToArray();
        //    this.exportItems = exportItems.ToArray();
        //} 
    }
}