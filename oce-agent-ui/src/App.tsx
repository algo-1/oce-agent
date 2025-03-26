import React from "react";
import { initializeIcons, Stack } from "@fluentui/react";
import IncidentsList from "./components/IncidentsList";
import AddTsgForm from "./components/uploadTsg";

initializeIcons();

const App: React.FC = () => {
  return (
    <Stack
      tokens={{ childrenGap: 50 }}
      verticalAlign="center"
      horizontalAlign="center"
      styles={{ root: { minHeight: "100vh", padding: 20 } }}
    >
      <AddTsgForm />
      <IncidentsList />
    </Stack>
  );
};

export default App;
