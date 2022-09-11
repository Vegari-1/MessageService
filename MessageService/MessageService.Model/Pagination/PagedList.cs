﻿using MongoDB.Driver;

namespace MessageService.Repository.Interface.Pagination;

public class PagedList<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public List<T> Items { get; set; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static PagedList<TProjected> ToPagedList<TProjected>(IFindFluent<T, TProjected> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source
                        .Skip((pageNumber - 1) * pageSize)
                        .Limit(pageSize)
                        .ToList();

        return new PagedList<TProjected>(items, (int)count, pageNumber, pageSize);
    }

    public PagedList<TMap> ToPagedList<TMap>(List<TMap> list)
    {
        return new PagedList<TMap>(list, TotalCount, CurrentPage, PageSize);
    }

    public List<T> ToList()
    {
        return Items;
    }

    public PagedList() { }
}