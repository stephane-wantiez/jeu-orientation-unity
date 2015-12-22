using UnityEngine;

public class RepereType : MonoBehaviour
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

    public bool hasRegularValue()
    {
        if (type == TypeEnum.Couleur)
        {
            return typeCouleur < TypeCouleur.Passe;
        }
        else
        {
            return typePointCardinal < TypePointCardinal.Passe;
        }
    }

    public static TypeEnum getTypeForCurrentGame()
    {
        if (GameSettings.Instance.GameType == GameManager.GameType.OrientColor)
        {
            return TypeEnum.Couleur;
        }
        else
        {
            return TypeEnum.PointCardinal;
        }
    }

    public static Color getTypeCouleurValue(TypeCouleur typeCouleurToGet)
    {
        switch (typeCouleurToGet)
        {
            case TypeCouleur.Rouge: return Color.red;
            case TypeCouleur.Bleu: return Color.blue;
            case TypeCouleur.Vert: return Color.green;
            case TypeCouleur.Jaune: return Color.yellow;
            default: return TRANSPARENT_COLOR;
        }
    }

    public Color getTypeCouleurValue()
    {
        return getTypeCouleurValue(typeCouleur);
    }

    public static string getTypePointCardinalValue(TypePointCardinal typePointCardinalToGet, bool returnSpecialAsTresor)
    {
        switch (typePointCardinalToGet)
        {
            case TypePointCardinal.Nord: return Localization.Get(LOCKEY_CARDINAL_NORD);
            case TypePointCardinal.Est: return Localization.Get(LOCKEY_CARDINAL_EST);
            case TypePointCardinal.Sud: return Localization.Get(LOCKEY_CARDINAL_SUD);
            case TypePointCardinal.Ouest: return Localization.Get(LOCKEY_CARDINAL_OUEST);
            case TypePointCardinal.Special: return returnSpecialAsTresor ? Localization.Get(LOCKEY_CARDINAL_SPECIAL_TRESOR) : "";
            default: return "";
        }
    }

    public string getTypePointCardinalValue(bool returnSpecialAsTresor)
    {
        return getTypePointCardinalValue(typePointCardinal, returnSpecialAsTresor);
    }
}
