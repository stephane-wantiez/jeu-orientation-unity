using swantiez.unity.tools.utils;
using System.Collections.Generic;
using UnityEngine;

public class ReperesManager : MonoBehaviour
{
    private static ReperesManager _instance;
    public static ReperesManager Instance { get { return _instance; } }

    public RepereType[] typesReperes;

    private readonly Dictionary<RepereType.TypeCouleur, RepereType> reperesPerColor = new Dictionary<RepereType.TypeCouleur, RepereType>();
    private readonly Dictionary<RepereType.TypePointCardinal, RepereType> reperesPerCard = new Dictionary<RepereType.TypePointCardinal, RepereType>();

    public Transform repereTopPosition;
    public Transform repereRightPosition;
    public Transform repereBottomPosition;
    public Transform repereLeftPosition;

    public RepereType repereTop;
    public RepereType repereRight;
    public RepereType repereBottom;
    public RepereType repereLeft;

    void Awake()
    {
        _instance = this;
        initReperesTypes();
    }

    private void initReperesTypes()
    {
        reperesPerColor.Clear();
        reperesPerCard.Clear();

        if (typesReperes != null)
        {
            foreach(RepereType typeRepere in typesReperes)
            {
                if (typeRepere.type == RepereType.TypeEnum.Couleur)
                {
                    reperesPerColor.Add(typeRepere.typeCouleur, typeRepere);
                }
                else // type = PointCardinal
                {
                    reperesPerCard.Add(typeRepere.typePointCardinal, typeRepere);
                }
            }
        }
    }

    private void initReperesWithRandomColors()
    {
        RepereType.TypeCouleur[] colors = new RepereType.TypeCouleur[RepereType.NB_TYPE_REGULIER];
        for (int i = 0; i < RepereType.NB_TYPE_REGULIER; ++i)
        {
            colors[i] = (RepereType.TypeCouleur)i;
        }
        colors.ShuffleFY();

        repereTop    = reperesPerColor[colors[0]];
        repereRight  = reperesPerColor[colors[1]];
        repereBottom = reperesPerColor[colors[2]];
        repereLeft   = reperesPerColor[colors[3]];
    }

    private void initReperesWithRandomPointCardinal()
    {
        int pointCardinalHautValue = UnityEngine.Random.Range(0, RepereType.NB_TYPE_REGULIER);
        int pointCardinalDroiteValue = (pointCardinalHautValue + 1) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalBasValue    = (pointCardinalHautValue + 2) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalGaucheValue = (pointCardinalHautValue + 3) % RepereType.NB_TYPE_REGULIER;

        repereTop    = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalHautValue  ];
        repereRight  = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalDroiteValue];
        repereBottom = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalBasValue   ];
        repereLeft   = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalGaucheValue];
    }

    private void initReperes()
    {
        if (RepereType.getTypeForCurrentGame() == RepereType.TypeEnum.Couleur)
        {
            initReperesWithRandomColors();
        }
        else
        {
            initReperesWithRandomPointCardinal();
        }
    }

    private void generateRepere(RepereType repereType, Transform reperePosition)
    {
        Quaternion repereRotation = reperePosition.rotation;
        GameObject repereObject = (GameObject) Instantiate(repereType.gameObject, reperePosition.position, repereRotation);
        repereObject.transform.parent = reperePosition;
    }

    private void clearReperesParents()
    {
        repereTopPosition.DestroyAllChildren(false);
        repereRightPosition.DestroyAllChildren(false);
        repereBottomPosition.DestroyAllChildren(false);
        repereLeftPosition.DestroyAllChildren(false);
    }

    public void generateReperes()
    {
        initReperes();
        clearReperesParents();
        generateRepere(repereTop, repereTopPosition);
        generateRepere(repereRight, repereRightPosition);
        generateRepere(repereBottom, repereBottomPosition);
        generateRepere(repereLeft, repereLeftPosition);
    }

    public RepereType getRepereTypeWithIndex(int repereIndex)
    {
        switch (repereIndex)
        {
            case 0: return repereTop;
            case 1: return repereRight;
            case 2: return repereBottom;
            default: return repereLeft;
        }
    }

    public RepereType getCardinalRepereType(RepereType.TypePointCardinal cardinalType)
    {
        if (repereTop.type == RepereType.TypeEnum.Couleur) return null;
        if (repereTop.typePointCardinal == cardinalType) return repereTop;
        if (repereRight.typePointCardinal == cardinalType) return repereRight;
        if (repereBottom.typePointCardinal == cardinalType) return repereBottom;
        if (repereLeft.typePointCardinal == cardinalType) return repereLeft;
        return null;
    }
}
