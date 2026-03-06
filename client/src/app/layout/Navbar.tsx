import { Group } from "@mui/icons-material";
import { Box, AppBar, Toolbar, Typography, Container, MenuItem, LinearProgress } from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";
import { Observer } from "mobx-react-lite";
import { useStore } from "../../lib/hooks/useStore";
 
export default function NavBar( ) {

    const {uiStore} = useStore();
    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="static" 
                    sx={{ backgroundImage: 'linear-gradient(135deg, #470f0f 0%, #ae4221 69%, #b16015 89%)' 
                          , position: 'relative' 
            }}>
                <Container maxWidth='xl'>
                    <Toolbar sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Box>
                            <MenuItem  component={NavLink} to='/' sx={{ display: 'flex', gap: 2 }}>
                                <Group fontSize='large' />
                                <Typography variant="h4" fontWeight='bold'>Reactivities</Typography>
                            </MenuItem>
                        </Box>
                        <Box sx={{display: 'flex'}}>
                            <MenuItemLink  to='/activities'>
                                Activities
                            </MenuItemLink>
                            <MenuItemLink to='/createActivity'>
                                Create Activitiy
                            </MenuItemLink>
                            <MenuItemLink to='/counter'>
                                counter
                            </MenuItemLink>
                        </Box>
                           <MenuItem>
                                User Menu
                            </MenuItem>
                    </Toolbar>
                </Container>
                  <Observer>
                    {() =>
                        uiStore.isLoading ? (
                            <LinearProgress
                                color="secondary"
                                sx={{
                                    position: 'absolute',
                                    bottom: 0,
                                    left: 0,
                                    right: 0,
                                    height: 4, // Adjust height if needed
                                }}
                            />
                        ) : null
                    }
                </Observer>
            </AppBar>
        </Box>
    )
}
