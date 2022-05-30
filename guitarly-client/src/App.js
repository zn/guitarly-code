import React, { useState, useEffect, Fragment } from 'react';
import bridge from '@vkontakte/vk-bridge';

import {MODAL_TRANSPOSE, MODAL_SHARE_MOBILE, PANEL_SONG, PANEL_ARTIST, PANEL_TOP_ARTISTS, PANEL_NEW_SONGS, PANEL_TOP_SONGS, PANEL_MAIN,
		PANEL_SEARCH, PANEL_SAVED, PANEL_EDIT_SONG, PANEL_UNPUBLISHED_SONGS,
	 VIEW_MAIN, VIEW_SAVED, VIEW_SEARCH, PAGE_MAIN, PAGE_SEARCH, PAGE_SAVED, PANEL_ADD_SONG, PANEL_ADD_ARTIST } from './routers';
import { useLocation, useRouter } from '@happysanta/router';

import { ModalPage, Panel, Placeholder, Button, ModalRoot,ModalPageHeader,PanelHeaderClose, View, ScreenSpinner, AdaptivityProvider,SplitCol, SplitLayout, AppRoot, Epic, Tabbar, TabbarItem } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';
import { usePlatform, ANDROID, IOS } from '@vkontakte/vkui';
import { auth } from './utils';

import { Icon56WifiOutline, Icon28NewsfeedOutline,Icon28SearchOutline ,Icon28NotebookCheckOutline, Icon56ErrorOutline} from '@vkontakte/icons';
import Home from './panels/Home';
import Song from './panels/Songs/Song';
import Artist from './panels/Artists/Artist';
import TopArtists from './panels/Artists/TopArtists';
import TopSongs from './panels/Songs/TopSongs';
import NewSongs from './panels/Songs/NewSongs';
import UnpublishedSongs from './panels/Songs/UnpublishedSongs';
import AppSearch from './panels/AppSearch';
import SavedItems from './panels/Songs/SavedItems';
import EditSong from './panels/Songs/EditSong';
import AddSong from './panels/Songs/AddSong';
import AddArtist from './panels/Artists/AddArtist';

import { VK_APP_URL } from './config';

const App = () => {
	const location = useLocation(true);
	const router = useRouter();
	const platform = usePlatform();
	const [fetchedUser, setUser] = useState(null);
	const [popout, setPopout] = useState(<ScreenSpinner size='large' />);
	const isMobile = window.innerWidth < 650; //viewWidth <= ViewWidth.MOBILE;

	const [selectedTab, setSelectedTab] = useState(VIEW_MAIN);

	const [modalShareMobile, setModalShareMobile] = useState(null);
	const [modalTranspose, setModalTranspose] = useState(null);

	const [isOnline, setIsOnline] = useState(true); 

	var modal = (
		<ModalRoot activeModal={location.getModalId()} onClose={() => router.popPage()}>
			<ModalPage id={MODAL_SHARE_MOBILE} onClose={() => router.popPage()}
				header={<ModalPageHeader left={<PanelHeaderClose onClick={() => router.popPage()} />}>
					Поделиться
				</ModalPageHeader>}>
				{modalShareMobile}
			</ModalPage>
			<ModalPage id={MODAL_TRANSPOSE} onClose={() => router.popPage()}
				header={<ModalPageHeader left={<PanelHeaderClose onClick={() => router.popPage()} />}>
					Транспонирование
				</ModalPageHeader>}>
				{modalTranspose}
			</ModalPage>
		</ModalRoot>
	)

	useEffect(() => {
		async function fetchData() {
			// if(isMobile){ // fixing a bug with mobile apps
			// 	await new Promise(r => setTimeout(r, 1200));
			// 	//await bridge.send("VKWebAppInit");
			// }

			const user = await bridge.send('VKWebAppGetUserInfo');
			setUser(user);
			await auth(user, platform);
			setPopout(null);
		}
		fetchData();
	}, []);

	function tabClick(page){
		if(location.getPageId() === page) return; // обрабатываем несколько кликов
		router.pushPage(page);

		if(page === PAGE_MAIN)
			setSelectedTab(VIEW_MAIN);
		else if(page === PAGE_SEARCH)
			setSelectedTab(VIEW_SEARCH);
		else if(page === PAGE_SAVED)
			setSelectedTab(VIEW_SAVED);
	}

	window.addEventListener('load', function() {
		function updateOnlineStatus(event) {
			var condition = navigator.onLine;
			console.log('condition is', condition);
			setIsOnline(condition);
		}
	  
		window.addEventListener('online',  updateOnlineStatus);
		window.addEventListener('offline', updateOnlineStatus);
	  });
	  
	return (
		<AdaptivityProvider>
			<AppRoot>
				{!isOnline &&
					<View activePanel='offline_placeholder_panel'>
					<Panel id='offline_placeholder_panel'>
					<Placeholder
						icon={<Icon56WifiOutline />}
						stretched
					>
						Нет подключения к интернету
					</Placeholder>
					</Panel>
				</View>}
				{isOnline && 
				<SplitLayout modal={modal}>
					<SplitCol>
						
				<Epic activeStory={location.getViewId()} tabbar={
					<Tabbar>
						<TabbarItem
							onClick={()=>tabClick(PAGE_MAIN)}
							selected={selectedTab === VIEW_MAIN}
							text="Главная"
						><Icon28NewsfeedOutline /></TabbarItem>
						<TabbarItem
							onClick={()=>tabClick(PAGE_SEARCH)}
							selected={selectedTab === VIEW_SEARCH}
							text="Поиск"
						><Icon28SearchOutline /></TabbarItem>
						<TabbarItem
							onClick={()=>tabClick(PAGE_SAVED)}
							selected={selectedTab === VIEW_SAVED}
							text="Песенник"
						><Icon28NotebookCheckOutline /></TabbarItem>
					</Tabbar>
				}>
					
					<View id={VIEW_MAIN}
						popout={popout}
						onSwipeBack={() => router.popPage()}
						history={location.hasOverlay() ? [] : location.getViewHistory(VIEW_MAIN)}
						activePanel={location.getViewActivePanel(VIEW_MAIN)}>
						{!popout &&
							<Home id={PANEL_MAIN}/>
						}
						<Song id={PANEL_SONG} setModalShareMobile={setModalShareMobile} setModalTranspose={setModalTranspose}/>
						<Artist id={PANEL_ARTIST} />
						<UnpublishedSongs id={PANEL_UNPUBLISHED_SONGS} />
						<TopSongs id={PANEL_TOP_SONGS} />
						<NewSongs id={PANEL_NEW_SONGS} />
						<TopArtists id={PANEL_TOP_ARTISTS} />
						<EditSong id={PANEL_EDIT_SONG} />
						<AddSong id={PANEL_ADD_SONG} />
						<AddArtist id={PANEL_ADD_ARTIST} />
					</View>
					<View id={VIEW_SEARCH}
						popout={popout}
						onSwipeBack={() => router.popPage()}
						history={location.hasOverlay() ? [] : location.getViewHistory(VIEW_SEARCH)}
						activePanel={location.getViewActivePanel(VIEW_SEARCH)}>
						<AppSearch id={PANEL_SEARCH} />
					</View>
					<View id={VIEW_SAVED}
						popout={popout}
						onSwipeBack={() => router.popPage()}
						history={location.hasOverlay() ? [] : location.getViewHistory(VIEW_SAVED)}
						activePanel={location.getViewActivePanel(VIEW_SAVED)}>
						<SavedItems id={PANEL_SAVED} />
					</View>
				</Epic>
				</SplitCol>
				</SplitLayout>
			}
			</AppRoot>
		</AdaptivityProvider>
	);
}

export default App;
