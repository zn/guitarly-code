import React, { useEffect, useState, Fragment } from 'react';
import PropTypes from 'prop-types';
import bridge from "@vkontakte/vk-bridge";

import { Panel, PanelHeader, Header, ScreenSpinner, Group, 
	List, Cell, Link, SimpleCell, PanelHeaderButton, Avatar, 
	PanelHeaderContext , HorizontalScroll, HorizontalCell, PanelHeaderContent, usePlatform } from '@vkontakte/vkui';

import { Icon16Dropdown, Icon28AddOutline, Icon12ChevronOutline } from '@vkontakte/icons';
import { Icon28ListAddOutline } from '@vkontakte/icons';
import { Icon28UserAddOutline } from '@vkontakte/icons';
import jwt_decode from "jwt-decode";

import { useRouter } from '@happysanta/router';
import { PAGE_SONG, PAGE_ARTIST, PAGE_NEW_SONGS, PAGE_TOP_ARTISTS, PAGE_TOP_SONGS, PAGE_UNPUBLISHED_SONGS, PAGE_ADD_SONG, PAGE_ADD_ARTIST } from '../routers';
import { declOfNum, getCookie, auth } from '../utils';
import { BASE_URL } from '../config';
import './Home.css';

const Home = ({ id }) => {
	
	const router = useRouter();
	const platform = usePlatform();
	const [feed, setFeed] = useState(null);
	let authToken = getCookie('auth_token', platform);
	const [isModer, setIsModer] = useState(false);
	const [contextOpened, setContextOpened] = useState(false);

	useEffect(() => {
		async function fetchData(){
			if(!authToken){ // handle bag with token
				const user = await bridge.send('VKWebAppGetUserInfo');
				await auth(user, platform);
				authToken = getCookie('auth_token', platform);
			}
			var userRole = jwt_decode(authToken).role;
			setIsModer(userRole === "admin" || userRole === "moder");
			const requestOptions = {
				headers: { 'Authorization': `Bearer ${authToken}` },
			};
			await fetch(BASE_URL + '/home/feed', requestOptions)
				.then(response => response.json())
				.then(data => setFeed(data))
				.catch(e => console.log(e));
		}
		
		fetchData();
	}, []);

	return(
	<Panel id={id}>
		<PanelHeader
			left={isModer &&
					<PanelHeaderButton onClick={()=>setContextOpened(!contextOpened)}>
						<Icon28AddOutline />
					</PanelHeaderButton>}
		>
			Guitarly
		</PanelHeader>
		{isModer && 
			<PanelHeaderContext opened={contextOpened} onClose={()=>setContextOpened(!contextOpened)}>
				<List>
					<Cell
						before={<Icon28ListAddOutline />}
						onClick={()=>router.pushPage(PAGE_ADD_SONG)}
					>
						Добавить песню 
					</Cell>
					<Cell
						before={<Icon28UserAddOutline />}
						onClick={()=>router.pushPage(PAGE_ADD_ARTIST)}
					>
						Добавить артиста
					</Cell>
				</List>
			</PanelHeaderContext>}
			
		{!feed && <ScreenSpinner/>}
		{feed &&
		<Fragment>
			{isModer && feed.unpublishedSongs.length > 0 &&
				<Group
					header={<Header mode="tertiary" aside={<Link onClick={()=>router.pushPage(PAGE_UNPUBLISHED_SONGS)}>Больше</Link>}>К ПУБЛИКАЦИИ</Header>}
				>
					{feed.unpublishedSongs.map(song=>
						<SimpleCell key={'unpublished_song_'+song.id} after={<Icon12ChevronOutline />} onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}>
							{song.fullTitle}
						</SimpleCell>)
					}
				</Group>
			}
			<Group
				header={<Header mode="tertiary" aside={<Link onClick={()=>router.pushPage(PAGE_NEW_SONGS)}>Больше</Link>}>НОВОЕ</Header>}
			>
				{feed.latestSongs.map(song=>
					<SimpleCell key={'latest_song_'+song.id} after={<Icon12ChevronOutline />} onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}>
						{song.fullTitle}
					</SimpleCell>)
				}
			</Group>

			<Group
				header={<Header mode="tertiary" aside={<Link onClick={()=>router.pushPage(PAGE_TOP_ARTISTS)}>Больше</Link>}>ТОП ИСПОЛНИТЕЛЕЙ</Header>}
			>
				<HorizontalScroll showArrows getScrollToLeft={i => i - 120} getScrollToRight={i => i + 120}>
					<div style={{ display: 'flex' }}>
						{feed.topArtists.map(artist=>
							<HorizontalCell key={'top_artist_'+artist.id} className='horizontal-cell'
											header={<div style={{fontSize:'16px',lineHeight:'20px', wordBreak:'break-word'}}>{artist.title}</div>}
											onClick={() => router.pushPage(PAGE_ARTIST, { artistId: artist.id })}>
								<Avatar size={100} mode='default' src={BASE_URL+artist.picture100} style={{objectFit:'cover'}}/>
							</HorizontalCell>
						)}
					</div>
				</HorizontalScroll>
			</Group>


			<Group
				header={<Header mode="tertiary" aside={<Link onClick={()=>router.pushPage(PAGE_TOP_SONGS, {})}>Больше</Link>}>ТОП ПЕСЕН</Header>}
			>
				{feed.topSongs.map((song, i) =>
					<SimpleCell
						key={'top_song_'+song.id}
						after={<Icon12ChevronOutline />}
						onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}
						description={song.totalViews + ' ' + declOfNum(song.viewsNumber, ['просмотр', 'просмотра', 'просмотров'])}>
						#{i+1} {song.fullTitle}
					</SimpleCell>
				)}
			</Group>
		</Fragment>}
	</Panel>
	);}

export default Home;
