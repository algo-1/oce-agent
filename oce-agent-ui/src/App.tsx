import React from "react";
import { initializeIcons, Stack } from "@fluentui/react";
import IncidentsList from "./components/IncidentsList";

initializeIcons();

const App: React.FC = () => {
  return (
    <Stack
      verticalAlign="center"
      horizontalAlign="center"
      styles={{ root: { minHeight: "100vh", padding: 20 } }}
    >
      <IncidentsList />
    </Stack>
  );
};

export default App;
