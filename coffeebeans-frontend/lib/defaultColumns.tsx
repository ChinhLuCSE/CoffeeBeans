import { GridColDef, GridRenderCellParams } from "@mui/x-data-grid";

export const defaultColumns: GridColDef[] = [
  {
    field: "id",
    headerName: "ID",
    width: 150,
    sortable: true,
    filterable: true,
  },
  {
    field: "tastingProfile",
    headerName: "Tasting Profile",
    width: 220,
    sortable: true,
    filterable: true,
    renderCell: (params: GridRenderCellParams) => (
      <div
        title={params.value as string}
        style={{
          overflow: "hidden",
          textOverflow: "ellipsis",
          whiteSpace: "nowrap",
        }}
      >
        {params.value as string}
      </div>
    ),
  },
  {
    field: "bagWeightG",
    headerName: "Bag Weight (g)",
    width: 150,
    sortable: true,
    filterable: true,
    valueFormatter: (params: { value: number | string | null }) =>
      typeof params.value === "number" ? params.value.toFixed(2) : params.value,
  },
  {
    field: "roastIndex",
    headerName: "Roast Index",
    width: 130,
    sortable: true,
    filterable: true,
    valueFormatter: (params: { value: number | string | null }) =>
      typeof params.value === "number" ? params.value.toFixed(2) : params.value,
  },
  {
    field: "numFarms",
    headerName: "Farms",
    width: 110,
    sortable: true,
    filterable: true,
  },
  {
    field: "numAcidityNotes",
    headerName: "Acidity Notes",
    width: 140,
    sortable: true,
    filterable: true,
  },
  {
    field: "numSweetnessNotes",
    headerName: "Sweetness Notes",
    width: 160,
    sortable: true,
    filterable: true,
  },
  {
    field: "x",
    headerName: "X",
    width: 100,
    sortable: true,
    filterable: true,
    valueFormatter: (params: { value: number | string | null }) =>
      typeof params.value === "number" ? params.value.toFixed(2) : params.value,
  },
  {
    field: "y",
    headerName: "Y",
    width: 100,
    sortable: true,
    filterable: true,
    valueFormatter: (params: { value: number | string | null }) =>
      typeof params.value === "number" ? params.value.toFixed(2) : params.value,
  },
];
