import React from "react";
import { initializeIcons } from "@fluentui/react";
import IncidentsList from "./components/IncidentsList";

initializeIcons();

const App: React.FC = () => {
  return (
    <div>
      <IncidentsList />
    </div>
  );
};

export default App;
