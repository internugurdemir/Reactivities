import { Group } from "@mui/icons-material";
import { Box, AppBar, Toolbar, Typography, Container, MenuItem, CircularProgress } from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";
import { Observer } from "mobx-react-lite";
import { useStore } from "../../lib/hooks/useStore";
import { useAccount } from "../../lib/hooks/useAccount.ts";
import UserMenu from "./UserMenu.tsx";
 
export default function NavBar( ) {

    const {uiStore} = useStore();
    const {currentUser} = useAccount();
    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="fixed" 
                    sx={{ backgroundImage: 'linear-gradient(135deg, #470f0f 0%, #ae4221 69%, #b16015 89%)' 
            }}>
                <Container maxWidth='xl'>
                    <Toolbar sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Box>
                            <MenuItem  component={NavLink} to='/' sx={{ display: 'flex', gap: 2 }}>
                                <Group fontSize='large' />
                                <Typography sx={{position: 'relative'}} variant="h4" fontWeight='bold'>Reactivities</Typography>
                                <Observer>
                                    {() =>
                                        uiStore.isLoading ? (
                                            <CircularProgress
                                                size={20}
                                                thickness={7}
                                                sx={{
                                                    color: 'white',
                                                    position: 'absolute',
                                                    top: '30%',
                                                    left: '105%', 
                                                }}
                                            />
                                        ) : null
                                    }
                                </Observer>
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
                            <MenuItemLink to='/errors'>
                                Errors
                            </MenuItemLink>
                        </Box>
                        <Box display='flex' alignItems='center'>
                            {currentUser?.displayName ? (
                                <UserMenu />
                            ) : (
                                 <>
                                    <MenuItemLink to='/login'>Login</MenuItemLink>
                                    <MenuItemLink to='/register'>Register</MenuItemLink>
                                </>
                            )}
                        </Box>
                    </Toolbar>
                </Container>
            </AppBar>
        </Box>
    )
}
