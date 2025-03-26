import React, { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

import {
  DetailsList,
  IColumn,
  DefaultButton,
  Text,
  Stack,
  Panel,
  MessageBar,
  MessageBarType,
} from "@fluentui/react";

interface Incident {
  id: string;
  title: string;
  description: string;
  status: string;
  createdAt: Date;
  updatedAt: Date;
  severity: string;
  tsg: string | null;
  link: string;
}

const IncidentsList: React.FC = () => {
  const [incidents, setIncidents] = useState<Incident[]>([]);
  const [selectedIncident, setSelectedIncident] = useState<Incident | null>(
    null
  );

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/incidentHub")
      .build();

    connection
      .start()
      .catch((err) => console.error("Error connecting to SignalR:", err));

    connection.on("ReceiveIncident", (incident: Incident) => {
      setIncidents((prevIncidents) => [...prevIncidents, incident]);
    });

    return () => {
      connection.stop();
    };
  }, []);

  const copyTSGToClipboard = (tsg: string | null) => {
    if (!tsg) {
      alert("No TSG steps available.");
      return;
    }
    navigator.clipboard.writeText(tsg).then(
      () => {
        alert("TSG steps copied to clipboard!");
      },
      (err) => {
        console.error("Could not copy text: ", err);
      }
    );
  };

  const columns: IColumn[] = [
    {
      key: "id",
      name: "ID",
      fieldName: "id",
      minWidth: 50,
      maxWidth: 100,
      isResizable: true,
    },
    {
      key: "title",
      name: "Title",
      fieldName: "title",
      minWidth: 150,
      maxWidth: 300,
      isResizable: true,
    },
    {
      key: "severity",
      name: "Severity",
      fieldName: "severity",
      minWidth: 100,
      maxWidth: 150,
      isResizable: true,
    },
    {
      key: "status",
      name: "Status",
      fieldName: "status",
      minWidth: 100,
      maxWidth: 150,
      isResizable: true,
    },
    {
      key: "createdAt",
      name: "Created At",
      fieldName: "createdAt",
      minWidth: 150,
      maxWidth: 200,
      isResizable: true,
      onRender: (item: Incident) => new Date(item.createdAt).toLocaleString(),
    },
    {
      key: "view",
      name: "Actions",
      minWidth: 100,
      onRender: (item: Incident) => (
        <Stack horizontal tokens={{ childrenGap: 10 }}>
          <DefaultButton
            text="View"
            onClick={() => setSelectedIncident(item)}
          ></DefaultButton>
          <DefaultButton
            text="Open Incident"
            href={item.link}
            target="_blank"
          ></DefaultButton>
        </Stack>
      ),
    },
  ];

  return (
    <div style={{ padding: 5 }}>
      <h1>Processed Incidents</h1>
      <DetailsList items={incidents} columns={columns} selectionMode={0} />
      {selectedIncident && (
        <Panel
          isOpen={!!selectedIncident}
          onDismiss={() => setSelectedIncident(null)}
          headerText={`Incident Details - ${selectedIncident.title}`}
        >
          <Text>
            <strong>Severity:</strong> {selectedIncident.severity}
          </Text>
          <Text>
            <strong>Status:</strong> {selectedIncident.status}
          </Text>

          {selectedIncident.tsg && (
            <>
              <MessageBar messageBarType={MessageBarType.info}>
                You can use the TSG below to aid mitigation.
              </MessageBar>
              <Text block>{selectedIncident.tsg}</Text>
            </>
          )}

          <Stack
            horizontal
            tokens={{ childrenGap: 10 }}
            style={{ marginTop: 20 }}
          >
            <DefaultButton
              text="Copy TSG Steps"
              onClick={() => copyTSGToClipboard(selectedIncident.tsg)}
            ></DefaultButton>
            <DefaultButton
              text="Open Incident"
              href={selectedIncident.link}
              target="_blank"
            ></DefaultButton>
            <DefaultButton
              text="Close"
              onClick={() => setSelectedIncident(null)}
            ></DefaultButton>
          </Stack>
        </Panel>
      )}
    </div>
  );
};

export default IncidentsList;
