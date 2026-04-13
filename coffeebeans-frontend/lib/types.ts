export type ColumnDataTypeType = "integer" | "double" | "string";

export interface ColumnData {
  name: string;
  type: ColumnDataTypeType;
}

export interface CustomColumn {
  id: string;
  name: string;
  key: string;
  dataType: ColumnDataTypeType;
}

export interface Bean {
  id: string;
  tastingProfile: string;
  bagWeightG: number;
  roastIndex: number;
  numFarms: number;
  numAcidityNotes: number;
  numSweetnessNotes: number;
  x: number;
  y: number;
  [key: string]: number | string | null | undefined;
}

export interface BeansResponse {
  items: Bean[];
  customColumns: CustomColumn[];
  page: number;
  perPage: number;
  totalCount: number;
  totalPages: number;
}
