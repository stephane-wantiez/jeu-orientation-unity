﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReperesManager : MonoBehaviour
{
    private static ReperesManager _instance;
    public static ReperesManager Instance { get { return _instance; } }

    public RepereType[] typesReperes;

    private Dictionary<RepereType.TypeCouleur, RepereType> reperesPerColor = new Dictionary<RepereType.TypeCouleur, RepereType>();
    private Dictionary<RepereType.TypePointCardinal, RepereType> reperesPerCard = new Dictionary<RepereType.TypePointCardinal, RepereType>();

    public Transform repereHautPosition;
    public Transform repereDroitePosition;
    public Transform repereBasPosition;
    public Transform repereGauchePosition;

    private RepereType repereHaut;
    private RepereType repereDroite;
    private RepereType repereBas;
    private RepereType repereGauche;

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
        Utils.ShuffleFY(colors);

        repereHaut   = reperesPerColor[colors[0]];
        repereDroite = reperesPerColor[colors[1]];
        repereBas    = reperesPerColor[colors[2]];
        repereGauche = reperesPerColor[colors[3]];
    }

    private void initReperesWithRandomPointCardinal()
    {
        int pointCardinalHautValue = UnityEngine.Random.Range(0, RepereType.NB_TYPE_REGULIER);
        int pointCardinalDroiteValue = (pointCardinalHautValue + 1) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalBasValue    = (pointCardinalHautValue + 2) % RepereType.NB_TYPE_REGULIER;
        int pointCardinalGaucheValue = (pointCardinalHautValue + 3) % RepereType.NB_TYPE_REGULIER;

        repereHaut   = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalHautValue  ];
        repereDroite = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalDroiteValue];
        repereBas    = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalBasValue   ];
        repereGauche = reperesPerCard[(RepereType.TypePointCardinal)pointCardinalGaucheValue];
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
        GameObject repereObject = Instantiate(repereType.gameObject, reperePosition.position, repereRotation) as GameObject;
        repereObject.transform.parent = reperePosition;
    }

    private void clearReperesParents()
    {
        Utils.DeleteChildrenOfTransform(repereHautPosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereDroitePosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereBasPosition, DestroyImmediate);
        Utils.DeleteChildrenOfTransform(repereGauchePosition, DestroyImmediate);
    }

    public void generateReperes()
    {
        initReperes();
        clearReperesParents();
        generateRepere(repereHaut, repereHautPosition);
        generateRepere(repereDroite, repereDroitePosition);
        generateRepere(repereBas, repereBasPosition);
        generateRepere(repereGauche, repereGauchePosition);
    }

    public RepereType getRepereTypeWithIndex(int repereIndex)
    {
        switch (repereIndex)
        {
            case 0: return repereHaut;
            case 1: return repereDroite;
            case 2: return repereBas;
            default: return repereGauche;
        }
    }

    public RepereType getCardinalRepereType(RepereType.TypePointCardinal cardinalType)
    {
        if (repereHaut.type == RepereType.TypeEnum.Couleur) return null;
        if (repereHaut.typePointCardinal == cardinalType) return repereHaut;
        if (repereDroite.typePointCardinal == cardinalType) return repereDroite;
        if (repereBas.typePointCardinal == cardinalType) return repereBas;
        if (repereGauche.typePointCardinal == cardinalType) return repereGauche;
        return null;
    }
}
