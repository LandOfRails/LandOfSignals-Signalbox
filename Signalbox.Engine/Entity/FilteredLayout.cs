﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Signalbox.Engine.Entity;

public class FilteredLayout<T> : ILayout<T>
    where T : class, IStaticEntity
{
    private T? _selectedEntity;
    private readonly ILayout _layout;

    public event EventHandler? SelectionChanged;

    public event EventHandler? CollectionChanged;

    public T? SelectedEntity
    {
        get => _selectedEntity;
        set
        {
            _selectedEntity = value;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public FilteredLayout(ILayout layout)
    {
        _layout = layout;
        _layout.CollectionChanged += (s, e) => CollectionChanged?.Invoke(s, e);
    }

    public bool TryGet(int column, int row, [NotNullWhen(true)] out T? entity)
    {
        _layout.TryGet(column, row, out entity);
        return entity != null;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T entity in _layout.OfType<T>())
        {
            yield return entity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Remove(int column, int row)
    {
        _layout.Remove(column, row);
    }

    public void Set(int column, int row, T entity)
    {
        _layout.Set(column, row, entity);
    }

    public void Add(int column, int row, IEnumerable<IStaticEntityFactory<T>> entityFactories, int fromColumn, int fromRow)
    {
        T? entity = CreateNewStaticEntity(column, row, entityFactories, fromColumn, fromRow);

        if (entity is null)
        {
            return;
        }
        _layout.Add(column, row, entity);
    }

    private static T? CreateNewStaticEntity(int column, int row, IEnumerable<IStaticEntityFactory<T>> entityFactories, int fromColumn, int fromRow)
    {
        foreach (var factory in entityFactories)
        {
            if (factory.TryCreateEntity(column, row, fromColumn, fromRow, out var entity))
            {
                return entity;
            }
        }
        return null;
    }

    public bool IsAvailable(int column, int row)
    {
        _layout.TryGet(column, row, out IStaticEntity? entity);
        return entity == null || entity is T;
    }

    public ILayout ToLayout()
    {
        return _layout;
    }
}
