import { Group } from "@mui/icons-material";
import { Box, Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router";

export default function HomePage() {
  return (
    <Paper
      className="home"
      sx={{
        color: "white",
        display: "flex",
        flexDirection: "column",
        gap: 6,
        alignItems: "center",
        alignContent: "center",
        justifyContent: "center",
        height: "100vh",
        backgroundImage:
          "linear-gradient(135deg, rgb(165, 44, 44) 30%, #d4491f 69%, #f77f0f 79%)",
      }}
    >
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          alignContent: "center",
          color: "white",
          gap: 3,
        }}
      >
        <Group sx={{ height: 110, width: 110 }} />
        <Typography variant="h1" fontWeight="bold">
          Reactivities
        </Typography>
      </Box>
      <Typography variant="h2">Welcome to reactivities</Typography>
      <Button
        component={Link}
        to="/activities"
        size="large"
        variant="contained"
        sx={{ height: 80, borderRadius: 4, fontSize: "1.5rem",
              backgroundColor: "#fd9778",   // turuncu-kırmızı
                "&:hover": {
                  backgroundColor: "#e64a19"
                }
         }}
        
      >
        Take me to the activities!
      </Button>
    </Paper>
  );
}
