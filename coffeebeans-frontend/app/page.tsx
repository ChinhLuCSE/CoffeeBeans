"use client";

import { useState } from "react";
import { DataTable } from "./components/DataTable";
import { Container, Box, Typography, Button } from "@mui/material";
import AddColumnDialog from "./components/AddColumnDialog";
import AddIcon from "@mui/icons-material/Add";

export default function Home() {
  const [openAddDialog, setOpenAddDialog] = useState<boolean>(false);

  return (
    <>
      <Container>
        <Box display="flex" flexDirection="column" gap={4} my={4}>
          <Box
            display="flex"
            flexDirection={{ xs: "column", md: "row" }}
            justifyContent="space-between"
            alignItems={{ xs: "flex-start", md: "center" }}
            gap={2}
          >
            <Box
              display="flex"
              justifyContent="center"
              alignItems="center"
              bgcolor="white"
              px={2}
              py={0.5}
              borderRadius={1}
              border="1px solid #e0e0e0"
            >
              <Typography variant="h6" color="text.primary">
                RoastCraft Labs — Coffee Bean Database
              </Typography>
            </Box>
            <Button
              variant="contained"
              color="secondary"
              data-testid="open-add-column-dialog"
              onClick={() => setOpenAddDialog(true)}
              sx={{ textTransform: "none", borderRadius: 1 }}
              startIcon={<AddIcon />}
            >
              Add New Column
            </Button>
          </Box>
          <DataTable />
        </Box>
      </Container>
      <AddColumnDialog
        open={openAddDialog}
        onClose={() => setOpenAddDialog(false)}
      />
    </>
  );
}
