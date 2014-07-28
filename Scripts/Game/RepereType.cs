using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class RepereType
{
    public const int NB_TYPE_REGULIER = 4;
    public const int NB_TYPE_TOUS = 6;
    public const string LOCKEY_CARDINAL_EST = "cardinal_e";
    public const string LOCKEY_CARDINAL_OUEST = "cardinal_o";
    public const string LOCKEY_CARDINAL_NORD = "cardinal_n";
    public const string LOCKEY_CARDINAL_SUD = "cardinal_s";
    public const string LOCKEY_CARDINAL_SPECIAL_TRESOR = "dice_card_tresor";
    public static Color TRANSPARENT_COLOR = new Color(0, 0, 0, 0);

    public enum TypeEnum { Couleur, PointCardinal }
    public TypeEnum type;

    public enum TypeCouleur { Rouge, Bleu, Vert, Jaune, Passe, Multiple }
    public TypeCouleur typeCouleur;

    public enum TypePointCardinal { Nord, Est, Sud, Ouest, Passe, Special }
    public TypePointCardinal typePointCardinal;

    public GameObject reperePrefabHaut;
    public bool rotatePrefabAtGeneration;
    public GameObject deRepere;

    public Color getTypeCouleurValue()
    {
        switch (typeCouleur)
        {
            case TypeCouleur.Rouge: return Color.red;
            case TypeCouleur.Bleu: return Color.blue;
            case TypeCouleur.Vert: return Color.green;
            case TypeCouleur.Jaune: return Color.yellow;
            default: return TRANSPARENT_COLOR;
        }
    }

    public string getTypePointCardinalValue(bool returnSpecialAsTresor)
    {
        switch (typePointCardinal)
        {
            case TypePointCardinal.Nord: return Localization.Get(LOCKEY_CARDINAL_NORD);
            case TypePointCardinal.Est: return Localization.Get(LOCKEY_CARDINAL_EST);
            case TypePointCardinal.Sud: return Localization.Get(LOCKEY_CARDINAL_SUD);
            case TypePointCardinal.Ouest: return Localization.Get(LOCKEY_CARDINAL_OUEST);
            case TypePointCardinal.Special: return returnSpecialAsTresor ? Localization.Get(LOCKEY_CARDINAL_SPECIAL_TRESOR) : "";
            default: return "";
        }
    }
}
