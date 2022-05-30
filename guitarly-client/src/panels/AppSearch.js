import React, {Fragment, useState, useEffect} from 'react';
import PropTypes from 'prop-types';
import { useRouter, useLocation } from '@happysanta/router';

import { Link, Panel, PanelHeader, Search, Group, Footer, Header, Placeholder, ScreenSpinner, Avatar, SimpleCell } from '@vkontakte/vkui';
import { BASE_URL } from '../config';
import { Icon56SearchOutline, Icon12ChevronOutline } from '@vkontakte/icons';
import { PAGE_SONG, PAGE_ARTIST } from '../routers';

const AppSearch = ({ id }) => {
	
	const router = useRouter();
	const location = useLocation();
	var {q} = location.getParams(); 
	const [searchQuery, setSearchQuery] = useState('');
	const [searchResult, setSearchResult] = useState(null);
	const [searching, setSearching] = useState(false);

	useEffect(()=>{
		if(q){
			makeRequest(q);
		}
	},[])

	function makeRequest(query){
		setSearching(true);
		fetch(BASE_URL + '/home/search?q=' + encodeURIComponent(query))
			.then(response => response.json())
			.then(data => {
				setSearchResult(data);
				setSearching(false);
			});
	}

	var delayTimer;
	function doSearch(e) {
		clearTimeout(delayTimer);
		delayTimer = setTimeout(function() {			
			// Do the ajax stuff
			if(e.target.value.trim().length > 1 && /[\wа-яА-Я]+/.test(e.target.value)){
				location.route.setParams({"q":e.target.value});
				makeRequest(e.target.value.trim());
			}
		}, 1000); // Will do the ajax stuff after 1000 ms, or 1 s
	}

	return(
	<Panel id={id}>
		<PanelHeader>Поиск</PanelHeader>
		<Group>
            <Search defaultValue={q} onChange={doSearch} maxLength={50}/>  
			
			{searching && <ScreenSpinner />}
            
			{!searching && searchResult 
				&& searchResult.artists.length === 0 
				&& searchResult.songs.length === 0 
				&& <Footer style={{maxWidth:"300px", margin:"auto", marginTop:'50px'}}>
					<span style={{fontSize:'16px'}}>Ничего не найдено</span>
					<br/>
					<br/>
					Попробуйте изменить запрос или <Link href="https://vk.com/im?sel=-205329085" target="_blank">напишите</Link> нам, мы добавим интересующий подбор.
					</Footer>}

			{!searching && searchResult
				&& (searchResult.artists.length !== 0 || searchResult.songs.length !== 0)
				&& <Fragment>
					{searchResult.artists.length !== 0 &&
						<Group
							header={<Header mode="secondary">АРТИСТЫ</Header>}
						>
							{searchResult.artists.map(artist =>
								<SimpleCell key={'artist_' + artist.id} before={<Avatar src={BASE_URL+artist.picture100} width={40} />}
											after={<Icon12ChevronOutline />}
											onClick={() => router.pushPage(PAGE_ARTIST, { artistId: artist.id })}>
									{artist.title}
								</SimpleCell>)
							}
						</Group>}

					{searchResult.songs.length !== 0 &&
						<Group
							header={<Header mode="secondary">Песни</Header>}
						>
							{searchResult.songs.map(song =>
								<SimpleCell key={'song_' + song.id} after={<Icon12ChevronOutline />}
											onClick={() => router.pushPage(PAGE_SONG, { songId: song.id })}>
									{song.fullTitle}
								</SimpleCell>)
							}
						</Group>}
				</Fragment>}

			{!searching && !searchResult &&
				<Placeholder
					icon={<Icon56SearchOutline />}
				>
					Введите имя исполнителя <br/> или название песни
				</Placeholder>}
		</Group>
	</Panel>
	);}

export default AppSearch;
