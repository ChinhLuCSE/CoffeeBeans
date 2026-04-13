"use client";

import { useState } from "react";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import { API_BASE_URL } from "@/lib/api";
import { BeansResponse, ColumnData, ColumnDataTypeType, CustomColumn } from "@/lib/types";

export default function AddColumnDialog({
  open,
  onClose,
  onColumnAdded,
}: {
  open: boolean;
  onClose: () => void;
  onColumnAdded?: () => void;
}) {
  const [columnName, setColumnName] = useState("");
  const [dataType, setDataType] = useState<ColumnDataTypeType>("string");
  const [error, setError] = useState<string | null>(null);
  const queryClient = useQueryClient();

  const addColumnMutation = useMutation({
    mutationFn: async (columnData: ColumnData) => {
      const response = await fetch(`${API_BASE_URL}/Columns`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(columnData),
      });

      const responseData = await response.json().catch(() => null);

      if (!response.ok) {
        const errorMessage =
          responseData?.errors?.Name?.[0] ||
          responseData?.errors?.name?.[0] ||
          responseData?.errors?.Type?.[0] ||
          responseData?.errors?.type?.[0] ||
          responseData?.title ||
          responseData?.detail ||
          responseData?.message ||
          "Failed to add column";

        throw new Error(errorMessage);
      }

      return responseData as { message: string; column: CustomColumn };
    },
    onSuccess: (responseData, variables) => {
      queryClient.setQueriesData<BeansResponse>(
        { queryKey: ["beans"] },
        (previousData) => {
          if (!previousData) {
            return previousData;
          }

          const hasColumnAlready = previousData.customColumns.some(
            (column) => column.id === responseData.column.id
          );

          return hasColumnAlready
            ? previousData
            : {
                ...previousData,
                customColumns: [...previousData.customColumns, responseData.column],
              };
        }
      );

      queryClient.invalidateQueries({ queryKey: ["columns"] });
      queryClient.invalidateQueries({ queryKey: ["beans"] });

      setColumnName("");
      setDataType("string");
      setError(null);
      onClose();
      onColumnAdded?.();

      toast.success(`Column "${variables.name}" added and populated for beans!`, {
        hideProgressBar: true,
        icon() {
          return <CheckCircleIcon style={{ color: "#9031aa" }} />;
        },
      });
    },
    onError: (err) => {
      const errorMessage =
        err instanceof Error ? err.message : "An unexpected error occurred";

      toast.error(`Failed to add column: ${errorMessage}`, {
        hideProgressBar: true,
      });
      setError(errorMessage);
    },
  });

  const handleSubmit = () => {
    const trimmedColumnName = columnName.trim();

    if (!trimmedColumnName) {
      setError("Column name is required");
      return;
    }

    if (!/^[a-zA-Z0-9_]+$/.test(trimmedColumnName)) {
      setError(
        "Column name can only contain letters, numbers, and underscores"
      );
      return;
    }

    setError(null);
    addColumnMutation.mutate({
      name: trimmedColumnName,
      type: dataType,
    });
  };

  const handleClose = () => {
    setColumnName("");
    setDataType("string");
    setError(null);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add New Custom Bean Column</DialogTitle>
      <DialogContent>
        <Alert severity="info" sx={{ mb: 2, mt: 1 }}>
          New columns are created on the backend and randomized values are
          generated for all existing beans automatically.
        </Alert>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}
        <TextField
          slotProps={{
            htmlInput: { "data-testid": "column-name-input" },
          }}
          label="Column Name"
          value={columnName}
          size="small"
          onChange={(e) => setColumnName(e.target.value)}
          fullWidth
          margin="normal"
          helperText="Use only letters, numbers, and underscores"
          disabled={addColumnMutation.isPending}
        />
        <FormControl fullWidth margin="normal" variant="outlined" size="small">
          <InputLabel id="data-type-label">Data Type</InputLabel>
          <Select
            labelId="data-type-label"
            id="data-type-select"
            label="Data Type"
            value={dataType}
            onChange={(e) => setDataType(e.target.value as ColumnDataTypeType)}
            disabled={addColumnMutation.isPending}
          >
            <MenuItem value="integer">Integer</MenuItem>
            <MenuItem value="double">Double</MenuItem>
            <MenuItem value="string">String</MenuItem>
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button
          sx={{ textTransform: "none", color: "grey" }}
          onClick={handleClose}
          disabled={addColumnMutation.isPending}
        >
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          sx={{ textTransform: "none", borderRadius: 1 }}
          variant="contained"
          color="secondary"
          disabled={addColumnMutation.isPending}
        >
          {addColumnMutation.isPending ? "Adding..." : "Add Column"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
