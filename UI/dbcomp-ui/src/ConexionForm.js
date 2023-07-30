import React, { useState } from 'react';
import axios from 'axios';
import {Box, Grid,TextField, Button, Typography, Accordion, AccordionSummary, AccordionDetails, List, ListItem, ListItemText, ListSubheader  } from '@mui/material';
import './ConexionForm.css'; // Archivo de estilos CSS personalizados
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

const ConexionForm = () => {
  const [serverAddress, setServerAddress] = useState('');
  const [databaseName, setDatabaseName] = useState('');

  const [serverAddress2, setServerAddress2] = useState('');
  const [databaseName2, setDatabaseName2] = useState('');

  const [connectionStatus, setConnectionStatus] = useState('');
  const [connectionStatus2, setConnectionStatus2] = useState('');

  const [isConnected, setIsConnected] = useState(false);
const  [is1Connected, setIs1Connected] = useState(false);

const [tableDifferences, setTableDifferences] = useState([]);
//const  [is2Connected] = useState(false);
const  [is2Connected, setIs2Connected] = useState(false);
  const handleServerAddressChange = (event) => {
    setServerAddress(event.target.value);
  };

  const handleDatabaseNameChange = (event) => {
    setDatabaseName(event.target.value);
  };

  const [missingTablesInServer1, setMissingTablesInServer1 ] = useState('');
  const [missingTablesInServer2, setMissingTablesInServer2 ] = useState('');
  const [differentTablesFromServer, setdifferentTablesFromServer ] = useState('');
  const [differentTables, setdifferentTables ] = useState('');

  const [missingViewsInServer1, setMissingViewsInServer1 ] = useState('');
  const [missingViewsInServer2, setMissingViewsInServer2 ] = useState('');
  const [differentViewsFromServer, setdifferentViewsFromServer ] = useState('');
  const [differentViews, setdifferentViews ] = useState('');



  const handleConnect1 = async () => {
    try {
      axios.defaults.baseURL = 'https://localhost:7217/';
      const response = await axios.get( `/Server/serverAddressAndDb?serverAddress=${serverAddress}&databaseName=${databaseName}`,       
      );

      console.log(response.data); // Aquí puedes manejar la respuesta de la API según tus necesidades
     
      if(response.data)  
      {setConnectionStatus('Conexión exitosa'); 
      setIs1Connected(true);
      setIsConnected(is1Connected&&is2Connected);}
      else
       {setConnectionStatus('Conexión fallida');}
      
     
    } catch (error) {
      console.error('Error al verificar el servidor:', error);
      setConnectionStatus('Conexión fallida');
    }
  };

  const handleConnect2 = async () => {
    try {
      axios.defaults.baseURL = 'https://localhost:7217/';
      const response = await axios.get( `/Server/serverAddressAndDb?serverAddress=${serverAddress2}&databaseName=${databaseName2}`,
    //  const response = await axios.get('https://localhost:7217/Server/',
      
      );

      console.log(response.data); // Aquí puedes manejar la respuesta de la API según tus necesidades
      
      if(response.data)  
      {
       // alert(response.data);
        setConnectionStatus2('Conexión exitosa');
        setIs2Connected(true);
       
        setIsConnected(is1Connected&&is2Connected);
      
     }

      
      else
       {setConnectionStatus2('Conexión fallida');}
      
     
    } catch (error) {
      console.error('Error al verificar el servidor:', error);
      setConnectionStatus2('Conexión fallida');
    }
  };


  const handleCompare = async () => {
    try {
      axios.defaults.baseURL = 'https://localhost:7217/';
     // const response = await axios.get( `/Server/serverAddressAndDb?serverAddress=${serverAddress2}&databaseName=${databaseName2}`,
      const response =  await axios.get( `/Server/compare-tables?serverAddress1=${serverAddress}&databaseName1=${databaseName}&serverAddress2=${serverAddress2}&databaseName2=${databaseName2}`,
      );
     /*  {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          serverAddress: serverAddress,
          databaseName: databaseName,
          serverAddress2: serverAddress2,
          databaseName2: databaseName2,
        }),
      }); */
    console.log(response);
    /*   if (response.data.status ) {
        throw new Error('Error al comparar las tablas');
      } */

      //const result = await response.data.missingTablesInServer1.json();


     const data = response.data; // Accede directamente a la propiedad 'data' de la respuesta
     const missingTablesInServer1 = data.missingTablesInServer1;
     const missingTablesInServer2 = data.missingTablesInServer2;
     const differentTables = data.differentTables

     const missingViewsInServer1 = data.missingViewsInServer1;
     const missingViewsInServer2 = data.missingViewsInServer2;
     const differentViews = data.differentViews




     const differences = [...missingTablesInServer1, ...missingTablesInServer2, ...differentTables, ...missingViewsInServer1, ...missingViewsInServer2, ...differentViews];
      setTableDifferences(differences);

      setMissingTablesInServer1(missingTablesInServer1);
      setMissingTablesInServer2(missingTablesInServer2);
      setdifferentTablesFromServer(differentTables);
      setdifferentTables()

      setMissingViewsInServer1(missingViewsInServer1);
      setMissingViewsInServer2(missingViewsInServer2);
      setdifferentViewsFromServer(differentViews);
      setdifferentViews()


      // Acceder a los datos de las tablas faltantes
   





    //   setTableDifferences(missingTablesInServer2);
    } catch (error) {
      console.error(error);
    }
  };


  return (
    <div className="form-container">
       <Grid container spacing={2}>
        <Grid item xs={6}>
    <Typography variant="h6" gutterBottom className="form-title">Conexión 1</Typography>
    <TextField
      label="Servidor"
      value={serverAddress}
      onChange={e => setServerAddress(e.target.value)}
      fullWidth
      margin="normal"
      variant="outlined"
      className="form-input"
    />
    <TextField
      label="Base de datos"
      value={databaseName}
      onChange={e => setDatabaseName(e.target.value)}
      fullWidth
      margin="normal"
      variant="outlined"
      className="form-input"
    />
    <Button variant="contained" color="primary" className="form-button" onClick={handleConnect1}>Conectar</Button>
    <Typography variant="subtitle1" gutterBottom className="connection-status">{connectionStatus}</Typography>
    </Grid>
    <Grid item xs={6}>
    <Typography variant="h6" gutterBottom className="form-title">Conexión 2</Typography>
    <TextField
      label="Servidor"
      value={serverAddress2}
      onChange={e => setServerAddress2(e.target.value)}
      fullWidth
      margin="normal"
      variant="outlined"
      className="form-input"
    />
    <TextField
      label="Base de datos"
      value={databaseName2}
      onChange={e => setDatabaseName2(e.target.value)}
      fullWidth
      margin="normal"
      variant="outlined"
      className="form-input"
    />
    <Button variant="contained" color="primary" className="form-button" onClick={handleConnect2}>Conectar</Button>
    <Typography variant="subtitle1" gutterBottom  className="connection-status">{connectionStatus2}</Typography>
    </Grid>
      </Grid>
     
    {isConnected && (
        <Box display="flex" justifyContent="center" mt={2}>
          <Button variant="contained" color="primary" onClick={handleCompare}  className="form-button-compare">
            Comparar
          </Button>
        </Box>
      )}
       <br></br>
      {tableDifferences.length > 0 && 
      (
            
          


<Accordion className='custom-accordion'>
  <AccordionSummary className='custom-accordion-summary ' expandIcon={<ExpandMoreIcon />}>
    <Typography variant="h6">Tablas</Typography>
  </AccordionSummary>
  <AccordionDetails className="custom-accordion-details">
    <List >
      <ListSubheader>Faltantes en Servidor 1:</ListSubheader>
      {missingTablesInServer1.map((tableName, index) => (
        <ListItem  key={index}>
          <ListItemText primary={tableName} />
        </ListItem>
      ))}
      <ListSubheader>Faltantes en Servidor 2:</ListSubheader>
      {missingTablesInServer2.map((tableName, index) => (
        <ListItem key={index}>
          <ListItemText primary={tableName} />
        </ListItem>
      ))}
      <ListSubheader>Diferencias:</ListSubheader>
      {differentTablesFromServer.map((tableName, index) => (
        <ListItem key={index}>
          <ListItemText primary={tableName} />
        </ListItem>
      ))}
    </List>

  </AccordionDetails>
</Accordion>

 )}

<br></br>

{tableDifferences.length > 0 && 
      (
<Accordion className='custom-accordion'>
<AccordionSummary className='custom-accordion-summary2 ' expandIcon={<ExpandMoreIcon />}>
    <Typography variant="h6">Views</Typography>
  </AccordionSummary>
  <AccordionDetails className="custom-accordion-details2">
    <List >
      <ListSubheader>Faltantes en Servidor 1:</ListSubheader>
      {missingViewsInServer1.map((viewName, index) => (
        <ListItem  key={index}>
          <ListItemText primary={viewName} />
        </ListItem>
      ))}
      <ListSubheader>Faltantes en Servidor 2:</ListSubheader>
      {missingViewsInServer2.map((viewName, index) => (
        <ListItem key={index}>
          <ListItemText primary={viewName} />
        </ListItem>
      ))}
      <ListSubheader>Diferencias:</ListSubheader>
      {differentViewsFromServer.map((viewName, index) => (
        <ListItem key={index}>
          <ListItemText primary={viewName} />
        </ListItem>
      ))}
    </List>
  </AccordionDetails>
</Accordion>
      )},

  </div>
 
  );

};

export default ConexionForm;