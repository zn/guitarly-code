import React, { useState, useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';

import { Panel, PanelHeader, PanelHeaderButton, ScreenSpinner, Group, Spinner, Link, SimpleCell, CellButton, Div, Avatar, IconButton, HorizontalScroll, HorizontalCell, RichCell } from '@vkontakte/vkui';

import { IOS, Placeholder, platform } from '@vkontakte/vkui';

import { useFirstPageCheck, useParams, useRouter } from '@happysanta/router';
import { PAGE_ARTIST, PAGE_SONG } from '../../routers';
import InfiniteScroll from 'react-infinite-scroller';

import { Icon12View, Icon16MoreVertical, Icon12ChevronOutline, Icon24Back, Icon28ChevronBack } from '@vkontakte/icons';

import { BASE_URL } from '../../config';

import { declOfNum } from '../../utils';

const TopArtists = ({ id }) => {

	const osName = platform();
	const router = useRouter();
	const isFirstPage = useFirstPageCheck();
	const [artists, setArtists] = useState();
	const [hasMore, setHasMore] = useState(false);

	function fetchData(page) {
		fetch(BASE_URL + '/artists/top?page=' + page)
			.then(response => response.json())
			.then(data => {
				setArtists(artists ? artists.concat(data) : data);
				if (data.length === 0) {
					setHasMore(false);
				} else {
					setHasMore(true);
				}
			});
	}

	useEffect(() => {
		fetchData(1);
	}, []);

	return (
		<Panel id={id}>
			<PanelHeader
				left={<PanelHeaderButton onClick={() => {
					if (isFirstPage) {
						router.replacePage(PAGE_MAIN)
					} else {
						router.popPage()
					}
				}}
					style={{ backgroundColor: 'transparent' }}>
					{osName === IOS ? <Icon28ChevronBack /> : <Icon24Back />}
				</PanelHeaderButton>}
			>
				Топ исполнителей
			</PanelHeader>
			{!artists && <ScreenSpinner />}
			{artists &&
				<Div id="scrollableDiv">
					<InfiniteScroll
						pageStart={1}
						loadMore={fetchData}
						hasMore={hasMore}
						loader={<Spinner size="small" />}
						scrollableTarget="scrollableDiv"
					>
						{artists.map((artist, i) => (
							<SimpleCell
								key={'artist_' + artist.id}
								after={<Icon12ChevronOutline />}
								onClick={() => router.pushPage(PAGE_ARTIST, { artistId: artist.id })}
								description={artist.totalViews + ' ' + declOfNum(artist.totalViews, ['просмотр', 'просмотра', 'просмотров'])}>
								#{i + 1} {artist.title}
							</SimpleCell>
						))}
					</InfiniteScroll>
				</Div>
			}
		</Panel>
	);
}

export default TopArtists;
