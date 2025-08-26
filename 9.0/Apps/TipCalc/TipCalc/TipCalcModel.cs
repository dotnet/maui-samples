﻿using System.ComponentModel;

namespace TipCalc;

internal class TipCalcModel : INotifyPropertyChanged
{
    double subTotal, postTaxTotal, tipPercent, tipAmount, total;

    public event PropertyChangedEventHandler? PropertyChanged;

    public double SubTotal
    {
        set
        {
            if (subTotal != value)
            {
                subTotal = value;
                OnPropertyChanged(nameof(SubTotal));
                Recalculate();
            }
        }
        get
        {
            return subTotal;
        }
    }

    public double PostTaxTotal
    {
        set
        {
            if (postTaxTotal != value)
            {
                postTaxTotal = value;
                OnPropertyChanged(nameof(PostTaxTotal));
                Recalculate();
            }
        }
        get
        {
            return postTaxTotal;
        }
    }

    public double TipPercent
    {
        set
        {
            if (tipPercent != value)
            {
                tipPercent = value;
                OnPropertyChanged(nameof(TipPercent));
                Recalculate();
            }
        }
        get
        {
            return tipPercent;
        }
    }

    public double TipAmount
    {
        set
        {
            if (tipAmount != value)
            {
                tipAmount = value;
                OnPropertyChanged(nameof(TipAmount));
            }
        }
        get
        {
            return tipAmount;
        }
    }

    public double Total
    {
        set
        {
            if (total != value)
            {
                total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        get
        {
            return total;
        }
    }

    void Recalculate()
    {
        this.TipAmount = Math.Round(this.TipPercent * this.SubTotal / 100, 2);

        // Round total to nearest quarter.
        this.Total = Math.Round(4 * (this.PostTaxTotal + this.TipAmount)) / 4;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
