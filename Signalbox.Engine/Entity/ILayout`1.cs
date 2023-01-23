﻿using System.Diagnostics.CodeAnalysis;

namespace Signalbox.Engine.Entity;

public interface ILayout<T> : IEnumerable<T>
    where T : class, IStaticEntity
{
    T? SelectedEntity { get; set; }

    event EventHandler SelectionChanged;

    event EventHandler CollectionChanged;

    bool TryGet(int column, int row, [NotNullWhen(true)] out T? entity);
    void Add(int column, int row, IEnumerable<IStaticEntityFactory<T>> entityFactories, int fromColumn, int fromRow);
    bool IsAvailable(int column, int row);
    void Set(int column, int row, T entity);
    void Remove(int column, int row);
    ILayout ToLayout();
}
