"use client";

import { API_BASE_URL } from "@/lib/api";
import { defaultColumns } from "@/lib/defaultColumns";
import { Bean, BeansResponse, CustomColumn } from "@/lib/types";
import { Box } from "@mui/material";
import {
  DataGrid,
  GridColDef,
  GridFilterModel,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { useQuery } from "@tanstack/react-query";
import { useCallback, useMemo, useRef, useState } from "react";

const DEFAULT_SORT_MODEL: GridSortModel = [{ field: "id", sort: "asc" }];

function toApiFieldName(fieldName?: string | null) {
  if (!fieldName) {
    return "id";
  }

  if (fieldName.startsWith("custom_")) {
    return fieldName;
  }

  return fieldName.replace(/[A-Z]/g, (match) => `_${match.toLowerCase()}`);
}

export function DataTable() {
  const activeRequestRef = useRef<AbortController | null>(null);

  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    page: 0,
    pageSize: 10,
  });
  const [sortModel, setSortModel] = useState<GridSortModel>(DEFAULT_SORT_MODEL);
  const [filterModel, setFilterModel] = useState<GridFilterModel>({
    items: [],
  });

  const buildBeansQueryParams = useCallback(() => {
    const sortField = sortModel[0]?.field ?? "id";
    const sortDirection = sortModel[0]?.sort ?? "asc";
    const filterParams: Record<string, string> = {};

    filterModel.items.forEach((item) => {
      if (
        item.field &&
        item.value !== undefined &&
        item.value !== null &&
        String(item.value).trim() !== ""
      ) {
        filterParams[toApiFieldName(item.field)] = String(item.value).trim();
      }
    });

    return new URLSearchParams({
      page: String(paginationModel.page + 1),
      per_page: String(paginationModel.pageSize),
      sort_by: toApiFieldName(sortField),
      sort_dir: sortDirection,
      ...filterParams,
    });
  }, [filterModel.items, paginationModel.page, paginationModel.pageSize, sortModel]);

  const { data: beansData, isLoading, isFetching, error } = useQuery<BeansResponse>({
    queryKey: [
      "beans",
      paginationModel.page,
      paginationModel.pageSize,
      sortModel,
      filterModel,
    ],
    queryFn: async () => {
      const params = buildBeansQueryParams();
      const requestUrl = `${API_BASE_URL}/Beans?${params.toString()}`;

      if (activeRequestRef.current) {
        activeRequestRef.current.abort();
      }

      const abortController = new AbortController();
      activeRequestRef.current = abortController;

      try {
        const response = await fetch(requestUrl, {
          signal: abortController.signal,
        });

        if (!response.ok) {
          throw new Error(`Failed to fetch beans (${response.status})`);
        }

        return (await response.json()) as BeansResponse;
      } finally {
        if (activeRequestRef.current === abortController) {
          activeRequestRef.current = null;
        }
      }
    },
    staleTime: 5000,
    refetchOnWindowFocus: false,
    retry: false,
    placeholderData: (previousData) => previousData,
  });

  const generateCustomColumns = useCallback(
    (customCols: CustomColumn[]): GridColDef[] =>
      customCols.map((col) => ({
        field: col.key,
        headerName: col.name,
        width: 150,
        sortable: true,
        filterable: true,
      })),
    []
  );

  const columns = useMemo(
    () => [
      ...defaultColumns,
      ...(beansData?.customColumns?.length
        ? generateCustomColumns(beansData.customColumns)
        : []),
    ],
    [beansData?.customColumns, generateCustomColumns]
  );

  const rows = useMemo<Bean[]>(() => beansData?.items ?? [], [beansData?.items]);
  const rowCount = beansData?.totalCount ?? 0;

  const handlePaginationModelChange = useCallback(
    (newModel: GridPaginationModel) => {
      setPaginationModel((prev) => {
        if (
          prev.page === newModel.page &&
          prev.pageSize === newModel.pageSize
        ) {
          return prev;
        }

        return newModel;
      });
    },
    []
  );

  const handleSortModelChange = useCallback((newModel: GridSortModel) => {
    setSortModel(newModel.length > 0 ? newModel : DEFAULT_SORT_MODEL);
    setPaginationModel((prev) => ({ ...prev, page: 0 }));
  }, []);

  const handleFilterModelChange = useCallback((newModel: GridFilterModel) => {
    setFilterModel(newModel);
    setPaginationModel((prev) => ({ ...prev, page: 0 }));
  }, []);

  return (
    <Box sx={{ height: 600, width: "100%", position: "relative" }}>
      <DataGrid
        rows={rows}
        columns={columns}
        pagination
        paginationMode="server"
        rowCount={rowCount}
        loading={isLoading || isFetching}
        getRowId={(row) => row.id}
        disableRowSelectionOnClick
        keepNonExistentRowsSelected
        pageSizeOptions={[10, 25, 50, 100]}
        paginationModel={paginationModel}
        onPaginationModelChange={handlePaginationModelChange}
        sortingMode="server"
        sortModel={sortModel}
        onSortModelChange={handleSortModelChange}
        filterMode="server"
        filterModel={filterModel}
        onFilterModelChange={handleFilterModelChange}
        sx={
          error
            ? {
                "& .MuiDataGrid-overlayWrapper": {
                  minHeight: 160,
                },
              }
            : undefined
        }
      />
    </Box>
  );
}
